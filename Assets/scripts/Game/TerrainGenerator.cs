using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Common
{
    [SerializeField]
    public class TerrainGenerator : MonoBehaviour
    {

        public int Size;
        public int StepsNumber;
        public int TryCount;
        public GameObject TerrainParent;
        [Range(0f, 1f)]
        public float ChanceMax;
        [Range(0f, 1f)]
        public float ChanceMin;
        [Range(0f, 1f)]
        public float Dispersion;

        public Cell[] CellTypes;
        public Cell ClearCell;

        private Cell[,] Data;
        private Cell[,] ExtendedData;

        private List<Cell> ActiveCells = new List<Cell>();
        private List<Cell> NewActiveCells = new List<Cell>();
        private List<Cell> NearCellBioms = new List<Cell>();
        private List<string> StaticCellNames = new List<string>();

        private List<GameObject> TerrainData = new List<GameObject>();

        public List<GameObject> Generate()
        {
            Data = new Cell[Size, Size];

            for (int i = 0; i < (CellTypes.Length - 1); i++)
            {
                if (i < 0) i = 0;
                Cell firstCell = CellTypes[i];
                Cell secondCell = CellTypes[i + 1];
                bool changePlaces = false;
                int firstMaxNear = (firstCell.MaxCellNear == 0) ? CellTypes.Length : firstCell.MaxCellNear;
                int secondMaxNear = (secondCell.MaxCellNear == 0) ? CellTypes.Length : secondCell.MaxCellNear;

                if (firstCell.MaxCellNear == 0 && secondCell.MaxCellNear == 0) continue;
                else if (firstCell.MaxCellNear == 0) continue;
                else if (secondCell.MaxCellNear == 0) changePlaces = true;
                else if (secondCell.MaxCellNear > firstCell.MaxCellNear) changePlaces = true;
                
                if(changePlaces)
                {
                    CellTypes[i] = secondCell;
                    CellTypes[i + 1] = firstCell;
                    i -= 2;
                }
            }

            foreach (Cell cell in CellTypes)
            {
                int CurrentCount = 0;
                while (1 == 1)
                {
                    if (CurrentCount > TryCount) return new List<GameObject>();
                    CurrentCount++;

                    int newI = Random.Range(0, Size);
                    int newJ = Random.Range(0, Size);

                    if (Data[newI, newJ] != null) continue;

                    Cell NewCell = new Cell(cell, newI, newJ);

                    NearCellBioms = new List<Cell>();
                    if(!CheckNearBioms(NewCell)) continue;

                    Data[newI, newJ] = NewCell;
                    PutCellPrefab(NewCell);
                    ActiveCells.Add(NewCell);
                    break;
                }
            }

            for (int step = 0; step < StepsNumber; step++)
            {
                foreach (Cell cell in ActiveCells)
                {
                    if (StaticCellNames.Contains(cell.BiomCellName)) continue;
                    if (cell.MinCount != 0 && cell.CurrentCount < cell.MinCount) continue;

                    int i = cell.I;
                    int j = cell.J;

                    if (i - 1 >= 0) Infection(cell, Data[(i - 1), j], (i - 1), j);
                    if (i + 1 < Size) Infection(cell, Data[(i + 1), j], (i + 1), j);
                    if (j - 1 >= 0) Infection(cell, Data[i, (j - 1)], i, (j - 1));
                    if (j + 1 < Size) Infection(cell, Data[i, (j + 1)], i, (j + 1));

                    if ((cell.CurrentCount + 1 >= cell.MaxCount) && cell.MaxCount != 0) StaticCellNames.Add(cell.BiomCellName);
                }
                ActiveCells.Clear();
                ActiveCells = new List<Cell>(NewActiveCells);
                NewActiveCells = new List<Cell>();
            }

            ExtendedData = new Cell[Size + 4, Size + 4];
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    ExtendedData[i + 2, j + 2] = Data[i, j];
                }
            }

            for (int i = 2; i < Size + 2; i++)
            {
                for (int j = 2; j < Size + 2; j++)
                {
                    if (ExtendedData[i, j] == null || ExtendedData[i, j].BiomCellName == ClearCell.BiomCellName) continue;

                    if (ExtendedData[(i - 1), j] == null) PutClearCellPrefab((i - 1), j);
                    if (ExtendedData[(i - 2), j] == null) PutClearCellPrefab((i - 2), j);

                    if (ExtendedData[(i + 1), j] == null) PutClearCellPrefab((i + 1), j);
                    if (ExtendedData[(i + 2), j] == null) PutClearCellPrefab((i + 2), j);

                    if (ExtendedData[i, (j - 1)] == null) PutClearCellPrefab(i, (j - 1));
                    if (ExtendedData[i, (j - 2)] == null) PutClearCellPrefab(i, (j - 2));

                    if (ExtendedData[i, (j + 1)] == null) PutClearCellPrefab(i, (j + 1));
                    if (ExtendedData[i, (j + 2)] == null) PutClearCellPrefab(i, (j + 2));

                    if (ExtendedData[(i - 1), (j - 1)] == null) PutClearCellPrefab((i - 1), (j - 1));
                    if (ExtendedData[(i - 1), (j + 1)] == null) PutClearCellPrefab((i - 1), (j + 1));
                    if (ExtendedData[(i + 1), (j - 1)] == null) PutClearCellPrefab((i + 1), (j - 1));
                    if (ExtendedData[(i + 1), (j + 1)] == null) PutClearCellPrefab((i + 1), (j + 1));
                }
            }

            return TerrainData;
        }

        bool CheckNearBioms(Cell CurrentCell)
        {
            if (ActiveCells.Count == 0) return true;
            if (NearCellBioms.Contains(CurrentCell)) return false;

            int NearThis = 0;
            foreach (Cell cell in ActiveCells)
            {
                if (CurrentCell == cell) continue;

                float Range = (float)Mathf.Sqrt(Mathf.Pow(Mathf.Abs(CurrentCell.I - cell.I), 2) + Mathf.Pow(Mathf.Abs(CurrentCell.J - cell.J), 2));
                float Factor = (cell.MaxCount != 0 && cell.MaxCount < StepsNumber) ? (float)cell.MaxCount / StepsNumber : 1;
                float Factor2 = (CurrentCell.MaxCount != 0 && CurrentCell.MaxCount < StepsNumber) ? (float)CurrentCell.MaxCount / StepsNumber : 1;

                if (Range < StepsNumber * 1.55 * (Factor + Factor2) / 2 && Range > StepsNumber * Dispersion * (Factor + Factor2) / 2) NearThis++;
                else if (Range < StepsNumber * Dispersion * (Factor + Factor2) / 2) return false;
            }
            if (CurrentCell.MaxCellNear != 0 && NearThis > CurrentCell.MaxCellNear || NearThis < 1) return false;

            NearCellBioms.Add(CurrentCell);

            if (NearCellBioms.Count == ActiveCells.Count) return true;


            foreach (Cell cell in ActiveCells)
            {
                if (cell == CurrentCell) continue;
                print(CurrentCell.BiomCellName + " -- " + cell.BiomCellName);
                float Range = (float)Mathf.Sqrt(Mathf.Pow(Mathf.Abs(CurrentCell.I - cell.I), 2) + Mathf.Pow(Mathf.Abs(CurrentCell.J - cell.J), 2));
                float Factor = (cell.MaxCount != 0 && cell.MaxCount < StepsNumber) ? (float)cell.MaxCount / StepsNumber : 1;
                float Factor2 = (CurrentCell.MaxCount != 0 && CurrentCell.MaxCount < StepsNumber) ? (float)CurrentCell.MaxCount / StepsNumber : 1;

                if (Range < StepsNumber * 1.55 * (Factor + Factor2) / 2 && Range > StepsNumber * Dispersion * (Factor + Factor2) / 2)
                {
                    if (CheckNearBioms(cell)) return true;
                }
            }

            if (NearCellBioms.Count == ActiveCells.Count) return true;
            return false;
        }

        void Infection(Cell firstCell, Cell secondCell, int i, int j)
        {
            bool success = false;
            if (secondCell == null) success = true;
            else if (firstCell.BiomCellName == secondCell.BiomCellName) return;
            else if (StaticCellNames.Contains(secondCell.BiomCellName)) return;
            else if (secondCell.MinCount == 0 && secondCell.CurrentCount < secondCell.MinCount) return;
            else
            {
                float firstStrength = firstCell.Strength;
                float secondStrength = secondCell.Strength;
                float firstRand = Random.Range(ChanceMin, ChanceMax);
                float secondRand = Random.Range(ChanceMin, ChanceMax);

                if (firstStrength * firstRand > secondStrength * secondRand)
                {
                    success = true;
                    DeleteCellPrefab(secondCell);
                }
            }

            if (success)
            {
                Cell NewCell = new Cell(firstCell, i, j);
                NewCell.CurrentCount = (firstCell.CurrentCount + 1);
                Data[i, j] = NewCell;
                PutCellPrefab(NewCell);
                NewActiveCells.Add(NewCell);
            }
        }

        void PutCellPrefab(Cell cell)
        {
            GameObject NewCell = GameObject.Instantiate(cell.prefab, new Vector3(cell.I * 10, 0, cell.J * 10), Quaternion.identity) as GameObject;
            NewCell.name = (cell.BiomCellName + " " + cell.I + " " + cell.J);
            TerrainData.Add(NewCell);
            NewCell.transform.SetParent(TerrainParent.transform);
        }

        void PutClearCellPrefab(int i, int j)
        {
            Cell cell = new Cell(ClearCell, (i - 2), (j - 2));
            ExtendedData[i, j] = cell;
            PutCellPrefab(cell);
        }

        void DeleteCellPrefab(Cell cell)
        {
            GameObject go = GameObject.Find(cell.BiomCellName + " " + cell.I + " " + cell.J);
            TerrainData.Remove(go);
            DestroyImmediate(go);
        }


    }

    [System.Serializable]
    public class Cell
    {
        public string BiomCellName;
        [Range(0f, 1f)]
        public float Strength;
        public int MaxCount = 0;
        public int MinCount = 0;
        public int MaxCellNear;
        public GameObject prefab;

        [HideInInspector]
        public int I;
        [HideInInspector]
        public int J;
        [HideInInspector]
        public int CurrentCount = 1;

        public Cell(string name, GameObject pref, int I, int J)
        {
            this.BiomCellName = name;
            this.Strength = 1f;
            this.MaxCount = 0;
            this.prefab = pref;
            this.I = I;
            this.J = J;
        }

        public Cell(Cell otherCell, int I, int J)
        {
            this.BiomCellName = otherCell.BiomCellName;
            this.Strength = otherCell.Strength;
            this.prefab = otherCell.prefab;
            this.MaxCount = otherCell.MaxCount;
            this.CurrentCount = otherCell.CurrentCount + 1;
            this.I = I;
            this.J = J;
        }
    }
}