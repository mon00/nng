using UnityEngine;
using System.Collections;

public class terraintest : MonoBehaviour {

	public TerrainData _terraindata;
	// Use this for initialization
	void Start () {
		 
		GameObject terrain = new GameObject();
		terrain = Terrain.CreateTerrainGameObject(_terraindata);
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
} 