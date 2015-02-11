using UnityEngine;
using System.Collections;

public class terraintest : MonoBehaviour {
	public TerrainData[] terraindata;
	public int i;
	void Start () {
		for (int a = 1; a <= i; a++)
		{
		
		GameObject terrain = Terrain.CreateTerrainGameObject(terraindata[Random.Range(1, terraindata.Length)]);

		}

	} 
}