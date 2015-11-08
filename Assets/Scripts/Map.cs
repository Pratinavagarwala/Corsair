using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour
{
	public GameObject enemyDeathInt;

	// Prefabs, GameObjects, and Textures
	public List<GameObject> enemyPrefabs;
	public List<GameObject> bossPrefabs;
	public List<GameObject> obstaclePrefabs;

	public Transform destinationTransform;
	public Texture healthTexture;
	public Node destinationNode;
	public Node[] enemySpawnNodes;

	// Grid/Node
	private ObjectManager objectManager;

	// Wave control attributes
	public List<Wave> upcomingWaves = new List<Wave> ();
	public List<Wave> currentWaves = new List<Wave> ();
	public float waveSpawnDelay = 25;
	private float nextWaveSpawnEvent;
	public bool playerTriggeredWave;

	[HideInInspector]
	public GoogleMobileAdsScript ad;

	void Awake()
	{
		objectManager = ObjectManager.GetInstance ();
		ScaleAndPlaceMap ();
		
		MakeWaves ();

		destinationNode = objectManager.NodeManager.GetDestinationNode();
		destinationTransform.position = destinationNode.UnityPosition;
		destinationTransform.position = new Vector3(destinationTransform.position.x, -.9f, destinationTransform.position.z);

		enemySpawnNodes = objectManager.NodeManager.GetSpawnNodes ();
	}
	
	// Use this for initialization
	void Start () {
		ad = GameObject.FindGameObjectWithTag("Ad").GetComponent<GoogleMobileAdsScript>();
		ad.RequestInterstitial ();

//		foreach (Node node in objectManager.NodeManager.nodes){
//			Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), node.UnityPosition, Quaternion.Euler(Vector3.zero));
//		}

		LoadMapTexture ();
		PlaceDoodads ();
	}

	/// <summary>
	/// Displays the grid.
	/// </summary>
	void OnGUI ()
	{
		foreach (EnemyBase Gob in objectManager.ThingsWithHealthBars()) {
			//Health Bar
			float healthRatio = (((float)Gob.Health) / ((float)Gob.maxHealth));
			if (healthRatio != 1) {
				Vector2 objSize = Gob.GetPixelSize ();
				float width = Gob.healthBarSize.x;
				float height = Gob.healthBarSize.y;
	
				width = width * healthRatio;
				Vector3 wantedPos = Camera.main.WorldToScreenPoint (Gob.transform.position);
	
				GUI.color = new Color (2 * (1 - healthRatio), 2 * healthRatio, 0);
				GUI.DrawTexture (new Rect (wantedPos.x - width / 2, Screen.height - wantedPos.y - objSize.y / 2, width, height), healthTexture);
			}
		}
	}

	/// <summary>
	/// Initializes the waves and choses random enemies for each wave.
	/// The first wave always consists of basic enemies.
	/// </summary>
	private void MakeWaves ()
	{
		int random;
		Wave wave;

        for (int x = 1; x < objectManager.gameState.numberOfWaves; x++)
        {
			if (x % 10 == 0) {
				random = UnityEngine.Random.Range ((int)BossType.Start+1, (int)BossType.Max);
				wave = new Wave ((BossType)random, 1f, 1);
			} else {
				random = UnityEngine.Random.Range ((int)EnemyType.Start+1, (int)EnemyType.Max);
				wave = new Wave ((EnemyType)random, 1f, 10);
			}
			
			upcomingWaves.Add (wave);
		}
	}

	/// <summary>
	/// Starts a wave from the upcomming Waves list.
	/// Ends the game when all the waves have been ran, all the enemies are dead, and the player didn't lose.
	/// </summary>
	public void SpawnWave ()
	{
		if (upcomingWaves.Count > 0) {
			Wave wave = upcomingWaves [0];
			upcomingWaves.RemoveAt (0);
			currentWaves.Add (wave);
			objectManager.gameState.waveCount++;
		}
	}
	
	/// <summary>
	/// Creates enemies.
	/// </summary>
	public void CreateEnemies ()
	{
		if (Time.time >= nextWaveSpawnEvent || playerTriggeredWave) {
			objectManager.WaveWheel.UpdateSpriteImages = true;

			playerTriggeredWave = false;
			nextWaveSpawnEvent = Time.time + waveSpawnDelay;
			SpawnWave ();
		}

		foreach (Wave wave in currentWaves) {

			if(objectManager.gameState.GameSpeed == GameSpeed.Paused)
			{
				wave.nextEnemySpawnEvent += Time.deltaTime;
			}
			else 
			{
				wave.nextEnemySpawnEvent -= (Time.deltaTime * (float)objectManager.gameState.GameSpeed) - Time.deltaTime;
			}

			if (Time.time >= wave.nextEnemySpawnEvent && wave.numberOfEnemies > 0) {
				GameObject enemy;
				int spawnIndex = UnityEngine.Random.Range(0, enemySpawnNodes.Length);

				if(wave.enemyType == EnemyType.Max){
					enemy = bossPrefabs [(int)wave.bossType];
					GameObject madeEnemy = enemy.GetObjectFromPool(enemy.gameObject.name , enemySpawnNodes[spawnIndex].UnityPosition, Quaternion.Euler (new Vector3 (90, 0, 0)));
					madeEnemy.name = enemy.name;
				}else{
					enemy = enemyPrefabs [(int)wave.enemyType];
					GameObject madeEnemy = enemy.GetObjectFromPool(enemy.gameObject.name , enemySpawnNodes[spawnIndex].UnityPosition, Quaternion.Euler (new Vector3 (90, 0, 0)));
					madeEnemy.name = enemy.name;
				}

				wave.nextEnemySpawnEvent = Time.time + wave.spawnDelay;
				wave.numberOfEnemies--;
			}
		}

		currentWaves.RemoveAll (a => a.numberOfEnemies <= 0);
	}

	public void Update ()
	{
		if(objectManager.gameState.gameStarted && !objectManager.gameState.gameOver){

			if(objectManager.gameState.GameSpeed == GameSpeed.Paused)
			{
				nextWaveSpawnEvent += Time.deltaTime;
			}
			else 
			{
				nextWaveSpawnEvent -= (Time.deltaTime * (float)objectManager.gameState.GameSpeed) - Time.deltaTime;
			}


			objectManager.gameState.nextWaveCountDown = (int)(nextWaveSpawnEvent - Time.time);
			CreateEnemies ();
		}
	}

	public void ScaleAndPlaceMap()
	{
		Node firstNode = objectManager.NodeManager.nodes [0, 0];
		Node lastNode = objectManager.NodeManager.nodes [objectManager.NodeManager.size_x - 1,
		                                                 objectManager.NodeManager.size_y - 1];
		
		Vector3 center = new Vector3 ((lastNode.UnityPosition.x + firstNode.UnityPosition.x) / 2f,
		                              firstNode.UnityPosition.y - 10,
		                              (lastNode.UnityPosition.z + firstNode.UnityPosition.z) / 2f);

		int xIndex = -1;
		int yIndex = -1;
		for(int x = 0; x<objectManager.MapData.Tiles.GetLength(0); x++)
		{
			for(int y = 0; y<objectManager.MapData.Tiles.GetLength(1); y++)
			{
				if(xIndex == -1 && objectManager.MapData.Tiles[x,y].isNode){
					xIndex = x;
					yIndex = y;
					
					x = objectManager.MapData.Tiles.GetLength(0);
					y = objectManager.MapData.Tiles.GetLength(1);
				}
			}
		}

		float textureSizeX = objectManager.MapData.Tiles.GetLength (0);
		float textureSizeY = objectManager.MapData.Tiles.GetLength (1);

		center.z = center.z + (((textureSizeX - objectManager.NodeManager.size_x) / 2f) - xIndex) * objectManager.MapData.NodeSize.y;
		center.x = center.x + (((textureSizeY - objectManager.NodeManager.size_y) / 2f) - yIndex) * (objectManager.MapData.NodeSize.x / (objectManager.MapData.IsIsoGrid?2:1));

		transform.position = center;
		transform.localScale = new Vector3 (((objectManager.MapData.Tiles.GetLength(0) + 1) * objectManager.MapData.NodeSize.x) / (objectManager.MapData.IsIsoGrid?2:1),
		                                    (objectManager.MapData.Tiles.GetLength(1)+.5f) * objectManager.MapData.NodeSize.y,
		                                    1);
	}

	public void PlaceDoodads()
	{
		for(int x = 0; x<objectManager.MapData.Tiles.GetLength(0); x++)
		{
			for(int y = 0; y<objectManager.MapData.Tiles.GetLength(1); y++)
			{
				Tile tile = objectManager.MapData.Tiles[x, y];

				if(tile.doodads.Count > 0)
				{
					foreach(GameObject doodad in tile.doodads)
					{
						Instantiate(doodad,
						            objectManager.NodeManager.GetNodeFromTileIndex(x, y).UnityPosition,
						            Quaternion.Euler(Vector3.zero));
					}
				}
			}
		}
	}

	public void LoadMapTexture()
	{
		Texture2D[] textures = Resources.LoadAll<Texture2D>(objectManager.MapData.MapName+"/mapTextures");
		StartCoroutine (Animate(textures));
	}

	IEnumerator Animate(Texture2D[] gridTextures) {
		int itr = 0;
		while(true)
		{
			GetComponent<Renderer>().sharedMaterial.mainTexture = gridTextures[itr%gridTextures.Length];
			itr++;
			yield return new WaitForSeconds(objectManager.MapData.AnimationSpeed);
		}
	}
}
