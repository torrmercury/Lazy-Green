using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public enum PieceType {
	Empty = 0,
	Porridge,
	DoublePorridge,
	OverflowPorridge,
	DoubleOverflowPorridge,
	OctoPot,
	LittlePot,
	LeftPipe,
	RightPipe,
	Rock,
	Stump
}

/// <summary>
/// A simple Level Loader for original Ogmo level editor levels (Ogmo 1.x)
/// Can also be extended for procedural levels
/// </summary>
public class Level {

	// Separate from the cell size so we can change the cell size without needing to change
	// the ogmo template
	public const int TILE_SIZE = 32;

	// Our mapping from ogmo positions to piece types
	// Needs to be modified every time the ogmo tilemap is edited
	// Should have dimensions identical to the ogmo tilemap
	protected static PieceType[,] _tileMap = new PieceType[,] {
		{ PieceType.Porridge, PieceType.Rock, PieceType.Empty, PieceType.Empty },
		{ PieceType.DoublePorridge, PieceType.Stump, PieceType.Empty, PieceType.Empty },
		{ PieceType.OctoPot, PieceType.Empty, PieceType.Empty, PieceType.Empty },
		{ PieceType.RightPipe, PieceType.Empty, PieceType.Empty, PieceType.Empty },
		{ PieceType.LeftPipe, PieceType.Empty, PieceType.Empty, PieceType.Empty },
		{ PieceType.OverflowPorridge, PieceType.Empty, PieceType.Empty, PieceType.Empty },
		{ PieceType.DoubleOverflowPorridge, PieceType.Empty, PieceType.Empty, PieceType.Empty },
		{ PieceType.LittlePot, PieceType.Empty, PieceType.Empty, PieceType.Empty }
	};

	protected string _levelName;
	public string levelName {
		get { return _levelName; }
	}

	protected int _width, _height;

	protected int _numTurns = 5;

	protected PieceType[,] _bottomLayer, _midLayer, _topLayer;


	// Specific objects with params we'll need to spawn

	// The heroes
	protected bool _spawningJohann = false;
	protected int _johannStomach, _johannHp;
	protected Vector2 _johannGridPos;

	protected bool _spawningOlga = false;
	protected int _olgaStomach, _olgaHp;
	protected Vector2 _olgaGridPos;

	protected bool _spawningWerner = false;
	protected int _wernerStomach, _wernerHp;
	protected Vector2 _wernerGridPos;


	// Exits and entrances
	protected struct ExitEntranceDesc {
		public Vector2 gridPos;
		public int index;
		public ExitEntranceDesc(Vector2 gridPos, int index) {
			this.gridPos = gridPos;
			this.index = index;
		}
	}

	protected List<ExitEntranceDesc> _exits;
	protected List<ExitEntranceDesc> _entrances;


	// Items
	protected struct ChestDesc {
		public Vector2 gridPos;
		public string itemName;
		public ChestDesc(Vector2 gridPos, string itemName) {
			this.gridPos = gridPos;
			this.itemName = itemName;
		}
	}

	protected List<ChestDesc> _chests;



	public Level(string levelFileName) {
		_levelName = levelFileName;
		loadFromXml(levelFileName);
	}

	public virtual void loadFromLevel(HexGrid gridManager) {
		gridManager.gridWidth = _width;
		gridManager.gridHeight = _height;
		gridManager.initGrids();

		gridManager.turnsLeft = _numTurns;

		if (_spawningJohann && !Johann.johannSpawned) {
			Vector2 actualPos = gridManager.toActualCoord(_johannGridPos);
			GameObject johannObj = spawnGridPrefab(gridManager, gridManager.johannPrefab, actualPos);
			Johann johann = johannObj.GetComponent<Johann>();
			johann.currentHealth = _johannHp;
			johann.currentStomach = _johannStomach;
		}
		if (_spawningOlga && !Olga.olgaSpawned) {
			Vector2 actualPos = gridManager.toActualCoord(_olgaGridPos);
			GameObject olgaObj = spawnGridPrefab(gridManager, gridManager.olgaPrefab, actualPos);
			Olga olga = olgaObj.GetComponent<Olga>();
			olga.currentHealth = _olgaHp;
			olga.currentStomach = _olgaStomach;
		}
		if (_spawningWerner && !Werner.wernerSpawned) {
			Vector2 actualPos = gridManager.toActualCoord(_wernerGridPos);
			GameObject wernerObj = spawnGridPrefab(gridManager, gridManager.wernerPrefab, actualPos);
			Werner werner = wernerObj.GetComponent<Werner>();
			werner.currentHealth = _wernerHp;
			werner.currentStomach = _wernerStomach;
		}

		foreach (ExitEntranceDesc exit in _exits) {
			Vector2 actualPos = gridManager.toActualCoord(exit.gridPos);
			GameObject exitObj = spawnGridPrefab(gridManager, gridManager.exitPrefab, actualPos);
			exitObj.GetComponent<ExitHex>().exitIndex = exit.index;
			ExitHex exitPiece = exitObj.GetComponent<ExitHex>();
			exitPiece.type = HexGridPiece.EXIT_TYPE;
			exitPiece.gridPos = gridManager.toGridCoord(exitPiece.x, exitPiece.y);
			gridManager.addToCurrentGrid(exitObj.GetComponent<ExitHex>());
		}

		foreach (ExitEntranceDesc entrance in _entrances) {
			Vector2 actualPos = gridManager.toActualCoord(entrance.gridPos);
			GameObject entranceObj = spawnGridPrefab(gridManager, gridManager.entrancePrefab, actualPos);
			entranceObj.GetComponent<EntranceHex>().entranceIndex = entrance.index;
		}

		foreach (ChestDesc chest in _chests) {
			Vector2 actualPos = gridManager.toActualCoord(chest.gridPos);
			// Figure out what to spawn based on the name
			GameObject chestPrefab = null;
			if (chest.itemName == "potion") {
				chestPrefab = gridManager.potionPrefab;
			}
			else if (chest.itemName == "hpup") {
				chestPrefab = gridManager.hpUpPrefab;
			}
			else if (chest.itemName == "stoup") {
				chestPrefab = gridManager.stoUpPrefab;
			}
			else if (chest.itemName == "boots") {
				chestPrefab = gridManager.bootsPrefab;
			}

			if (chestPrefab != null)
				spawnGridPrefab(gridManager, chestPrefab, actualPos);
		}


		for (int i = 0; i < _width; i++) {
			for (int j = 0; j < _height; j++) {
				Vector2 pos = gridManager.toActualCoord(new Vector2(i, j));
				PieceType[] potentialObjects = new PieceType[] { _bottomLayer[i, j], _midLayer[i, j], _topLayer[i, j] };
				foreach (PieceType type in potentialObjects) {
					spawnItem(gridManager, pos, type);
				}
			}
		}
	}

	public static GameObject spawnGridPrefab(HexGrid gridManager, GameObject prefab, Vector2 pos) {
		GameObject newObj = GameObject.Instantiate(prefab) as GameObject;
		newObj.transform.parent = gridManager.transform;
		newObj.transform.localScale = Vector3.one;
		HexGridPiece piece = newObj.GetComponent<HexGridPiece>();
		piece.x = pos.x;
		piece.y = pos.y;
		return newObj;
	}


	public static void spawnItem(HexGrid gridManager, Vector2 pos, PieceType type) {
		// Just choose the correct prefab to spawn
		GameObject objToSpawn = null;
		if (type == PieceType.Porridge) {
			objToSpawn = gridManager.porridgePrefab;
		}
		else if (type == PieceType.DoublePorridge) {
			objToSpawn = gridManager.doublePorridgePrefab;
		}
		else if (type == PieceType.OverflowPorridge) {
			objToSpawn = gridManager.overflowPorridgePrefab;
		}
		else if (type == PieceType.DoubleOverflowPorridge) {
			objToSpawn = gridManager.doubleOverflowPorridgePrefab;
		}
		else if (type == PieceType.LittlePot) {
			objToSpawn = gridManager.littlePotPrefab;
		}
		else if (type == PieceType.OctoPot) {
			objToSpawn = gridManager.octoPotPrefab;
		}
		else if (type == PieceType.RightPipe) {
			objToSpawn = gridManager.rightPipePrefab;
		}
		else if (type == PieceType.LeftPipe) {
			objToSpawn = gridManager.leftPipePrefab;
		}
		else if (type == PieceType.Rock) {
			objToSpawn = gridManager.rockPrefab;
		}
		else if (type == PieceType.Stump) {
			objToSpawn = gridManager.stumpPrefab;
		}

		if (objToSpawn != null) {
			GameObject spawnedObj = GameObject.Instantiate(objToSpawn) as GameObject;
			Vector3 objLocalScale = spawnedObj.transform.localScale;
			spawnedObj.transform.parent = gridManager.transform;
			spawnedObj.transform.localScale = objLocalScale;
			HexGridPiece piece = spawnedObj.GetComponent<HexGridPiece>();
			piece.x = pos.x;
			piece.y = pos.y;
		}
	}

	public virtual void loadFromXml(string xmlFileName) {
		// First parse the xml
		TextAsset xmlText = Resources.Load(xmlFileName, typeof(TextAsset)) as TextAsset;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(xmlText.text);

		// Find the width and height of the level
		int width = 0;
		foreach (XmlElement widthElement in xmlDoc.GetElementsByTagName("width")) {
			width = int.Parse(widthElement.FirstChild.InnerText);
		}
		_width = width/TILE_SIZE;
		int height = 0;
		foreach (XmlElement heightElement in xmlDoc.GetElementsByTagName("height")) {
			height = int.Parse(heightElement.FirstChild.InnerText);
		}
		_height = height/TILE_SIZE;

		// Set up the three layers
		_bottomLayer = new PieceType[_width, _height];
		_midLayer = new PieceType[_width, _height];
		_topLayer = new PieceType[_width, _height];
		for (int i = 0; i < _width; i++) {
			for (int j = 0; j < _height; j++) {
				_bottomLayer[i, j] = PieceType.Empty;
				_midLayer[i, j] = PieceType.Empty;
				_topLayer[i, j] = PieceType.Empty;
			}
		}

		XmlNodeList dataList;
		// Bottom Layer
		foreach (XmlElement bottomElement in xmlDoc.GetElementsByTagName("bottom")) {
			dataList = bottomElement.GetElementsByTagName("tile");
			foreach (XmlElement dataElement in dataList) {
				int tileX = int.Parse(dataElement.GetAttribute("x")) / TILE_SIZE;
				int tileY = int.Parse(dataElement.GetAttribute("y")) / TILE_SIZE;
				// Have to flip the y coordinate b/c ogmo uses a different coord space
				tileY = _height - tileY - 1;

				PieceType type = getType(int.Parse(dataElement.GetAttribute("tx")), int.Parse(dataElement.GetAttribute("ty")));
				if (tileX >= 0 && tileX < _width && tileY >= 0 && tileY < _height)
					_bottomLayer[tileX, tileY] = type;

			}
		}
		// Mid Layer
		foreach (XmlElement bottomElement in xmlDoc.GetElementsByTagName("mid")) {
			dataList = bottomElement.GetElementsByTagName("tile");
			foreach (XmlElement dataElement in dataList) {
				int tileX = int.Parse(dataElement.GetAttribute("x")) / TILE_SIZE;
				int tileY = int.Parse(dataElement.GetAttribute("y")) / TILE_SIZE;
				// Have to flip the y coordinate b/c ogmo uses a different coord space
				tileY = _height - tileY - 1;
				
				PieceType type = getType(int.Parse(dataElement.GetAttribute("tx")), int.Parse(dataElement.GetAttribute("ty")));
				if (tileX >= 0 && tileX < _width && tileY >= 0 && tileY < _height)
					_bottomLayer[tileX, tileY] = type;
				
			}
		}
		// Top Layer
		foreach (XmlElement bottomElement in xmlDoc.GetElementsByTagName("top")) {
			dataList = bottomElement.GetElementsByTagName("tile");
			foreach (XmlElement dataElement in dataList) {
				int tileX = int.Parse(dataElement.GetAttribute("x")) / TILE_SIZE;
				int tileY = int.Parse(dataElement.GetAttribute("y")) / TILE_SIZE;
				// Have to flip the y coordinate b/c ogmo uses a different coord space
				tileY = _height - tileY - 1;
				
				PieceType type = getType(int.Parse(dataElement.GetAttribute("tx")), int.Parse(dataElement.GetAttribute("ty")));
				if (tileX >= 0 && tileX < _width && tileY >= 0 && tileY < _height)
					_bottomLayer[tileX, tileY] = type;
				
			}
		}

		// Now look for specific objects to spawn
		foreach (XmlElement johannElem in xmlDoc.GetElementsByTagName("johann")) {
			_spawningJohann = true;
			int tileX = int.Parse(johannElem.GetAttribute("x")) / TILE_SIZE;
			int tileY = int.Parse(johannElem.GetAttribute("y")) / TILE_SIZE;
			tileY = _height - tileY - 1;
			_johannHp = int.Parse(johannElem.GetAttribute("hp"));
			_johannStomach = int.Parse(johannElem.GetAttribute("stomach"));
			_johannGridPos = new Vector2(tileX, tileY);
		}
		foreach (XmlElement olgaElem in xmlDoc.GetElementsByTagName("olga")) {
			_spawningOlga = true;
			int tileX = int.Parse(olgaElem.GetAttribute("x")) / TILE_SIZE;
			int tileY = int.Parse(olgaElem.GetAttribute("y")) / TILE_SIZE;
			tileY = _height - tileY - 1;
			_olgaHp = int.Parse(olgaElem.GetAttribute("hp"));
			_olgaStomach = int.Parse(olgaElem.GetAttribute("stomach"));
			_olgaGridPos = new Vector2(tileX, tileY);
		}
		foreach (XmlElement wernerElem in xmlDoc.GetElementsByTagName("werner")) {
			_spawningWerner = true;
			int tileX = int.Parse(wernerElem.GetAttribute("x")) / TILE_SIZE;
			int tileY = int.Parse(wernerElem.GetAttribute("y")) / TILE_SIZE;
			tileY = _height - tileY - 1;
			_wernerHp = int.Parse(wernerElem.GetAttribute("hp"));
			_wernerStomach = int.Parse(wernerElem.GetAttribute("stomach"));
			_wernerGridPos = new Vector2(tileX, tileY);
		}

		// Now look for exits and entrances
		_exits = new List<ExitEntranceDesc>();
		foreach (XmlElement exitElem in xmlDoc.GetElementsByTagName("exit")) {
			int tileX = int.Parse(exitElem.GetAttribute("x")) / TILE_SIZE;
			int tileY = int.Parse(exitElem.GetAttribute("y")) / TILE_SIZE;
			tileY = _height - tileY - 1;
			int index = int.Parse(exitElem.GetAttribute("index"));
			ExitEntranceDesc exit = new ExitEntranceDesc(new Vector2(tileX, tileY), index);
			_exits.Add(exit);
		}
		_entrances = new List<ExitEntranceDesc>();
		foreach (XmlElement entranceElem in xmlDoc.GetElementsByTagName("entrance")) {
			int tileX = int.Parse(entranceElem.GetAttribute("x")) / TILE_SIZE;
			int tileY = int.Parse(entranceElem.GetAttribute("y")) / TILE_SIZE;
			tileY = _height - tileY - 1;
			int index = int.Parse(entranceElem.GetAttribute("index"));
			ExitEntranceDesc entrance = new ExitEntranceDesc(new Vector2(tileX, tileY), index);
			_entrances.Add(entrance);
		}


		_chests = new List<ChestDesc>();
		foreach (XmlElement chestElem in xmlDoc.GetElementsByTagName("chest")) {
			int tileX = int.Parse(chestElem.GetAttribute("x")) / TILE_SIZE;
			int tileY = int.Parse(chestElem.GetAttribute("y")) / TILE_SIZE;
			tileY = _height - tileY - 1;
			string itemName = chestElem.GetAttribute("item");
			ChestDesc chest = new ChestDesc(new Vector2(tileX, tileY), itemName);
			_chests.Add(chest);
		}

		// The settings object with different values
		foreach (XmlElement settingsElem in xmlDoc.GetElementsByTagName("settings")) {
			_numTurns = int.Parse(settingsElem.GetAttribute("numturns"));
		}


	}

	public static PieceType getType(int tx, int ty) {
		return _tileMap[ty / TILE_SIZE, tx / TILE_SIZE];
	}
	

}
