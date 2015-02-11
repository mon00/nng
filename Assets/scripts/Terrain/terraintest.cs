using UnityEngine;
using System.Collections;

public class terraintest : MonoBehaviour {
	public TerrainData[] terraindata;
	public int i;
	void Start () {
		if(terraindata.Length==0)
		{
			Debug.Log("No TerrainData in function");
			return;
		}
		for (int a = 1; a < i; a++)
		{		for (int b = 1; b < i; b++)
			{
		
		GameObject terrain = Terrain.CreateTerrainGameObject(terraindata[Random.Range(1, terraindata.Length)]);
				terrain.transform.position = new Vector3(100*a,0,100*b);
			}

		}	 
	}
}