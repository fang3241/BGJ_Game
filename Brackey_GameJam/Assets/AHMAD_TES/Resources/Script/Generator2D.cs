using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Graphs;

public class Generator2D : MonoBehaviour {
    [SerializeField]
    public enum CellType {
        None,
        Room,
        Hallway,
        RDoor,
        HDoor
    }

    class Room {
        public RectInt bounds;

        public Room(Vector2Int location, Vector2Int losize) {
            bounds = new RectInt(location, losize);
        }

        public static bool Intersect(Room a, Room b) {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }
    }
    [SerializeField]
    public GameObject camera;
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public static Vector2Int size;
    [SerializeField]
    Vector2Int losize;//40 x 40
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector2Int roomMaxSize;
    [SerializeField]
    Vector2Int roomMinSize;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    GameObject floor;
    [SerializeField]
    GameObject border;
    [SerializeField]
    GameObject wall;
    [SerializeField]
    Sprite floorMaterial;
    [SerializeField]
    Sprite borderMaterial;
    [SerializeField]
    Material greenMaterial;
    [SerializeField]
    Material yellowMaterial;
    [SerializeField]
    Sprite wallMaterial;
    [SerializeField]
    Transform wallParent;
    [SerializeField]
    Transform floorParent;
    [SerializeField]
    float scaling;
    [SerializeField]
    GameObject enemy;
    [SerializeField]
    int enemyCount;
    [SerializeField]
    Vector2Int tar;

    [SerializeField]
    public static Grid2D<CellType>[] grid = new Grid2D<CellType>[10];
    List<Room> rooms;
    Delaunay2D delaunay;
    HashSet<Prim.Edge> selectedEdges;
    //
    public int randSpawn;
    Vector2Int spawnPoint;
    GameObject wallP, floorP;
    GameObject[] Layer = new GameObject[10];
    


    void Start() {
        for(int i = 0; i < 10; i++){
            Generate(i);
        }
        changeLayer(9);
        GameObject _player = Instantiate(player, new Vector2(spawnPoint.x * scaling, (spawnPoint.y) * scaling), Quaternion.identity);
        _player.GetComponent<Transform>().localScale = new Vector2(scaling * 0.5f, scaling * 0.5f);
        camera.GetComponent<CameraMovement>()._target = _player.transform;
    }

    void Generate(int layer) {
        //Debug.Log("RandSpawn: " + randSpawn);
        //Debug.Log("cct" + Generator2D.CellType);
        Generator2D.grid[layer] = new Grid2D<CellType>(losize, Vector2Int.zero);
        Debug.Log("size" + losize);
        Generator2D.size = losize;
        Debug.Log("Gz" + Generator2D.size);
        Debug.Log("ggs"+ Generator2D.grid[layer][0,0]);
        rooms = new List<Room>();

        PlaceRooms(layer);
        Triangulate();
        CreateHallways();
        PathfindHallways(layer);
        randSpawn = Random.Range(0, (size.x + size.y)/2);
        Layer[layer] = new GameObject("Layer" + layer);
        wallP = new GameObject("wall");
        floorP = new GameObject("floor");
        wallP.transform.parent = Layer[layer].transform;
        floorP.transform.parent = Layer[layer].transform;
        Layer[layer].SetActive(false);
        drawFloor(layer);
        drawWall(layer);
        //generateEnemy(enemyCount);
    }

    void PlaceRooms(int layer) {
        for (int i = 0; i < roomCount; i++) {
            Vector2Int location = new Vector2Int(
                Random.Range(0, losize.x),
                Random.Range(0, losize.y)
            );

            Vector2Int roomSize = new Vector2Int(
                Random.Range(roomMinSize.x, roomMaxSize.x + 1),
                Random.Range(roomMinSize.y, roomMaxSize.y + 1)
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms) {
                if (Room.Intersect(room, buffer)) {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= losize.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= losize.y) {
                add = false;
            }
            if (add) {
                rooms.Add(newRoom);

                foreach (var pos in newRoom.bounds.allPositionsWithin) {
                    Debug.Log(pos);
                    Debug.Log(Generator2D.grid[layer].Size);
                    Debug.Log(Generator2D.grid[layer][0,0]);
                    Generator2D.grid[layer][pos] = CellType.Room;
                }
            }
        }
    }

    void Triangulate() {
        List<Vertex> vertices = new List<Vertex>();

        foreach (var room in rooms) {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay2D.Triangulate(vertices);
    }

    void CreateHallways() {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges) {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges) {
            if (Random.Range(1,8) == 8) {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways(int layer) {
        DungeonPathfinder2D aStar = new DungeonPathfinder2D(losize);

        foreach (var edge in selectedEdges) {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder2D.Node a, DungeonPathfinder2D.Node b) => {
                var pathCost = new DungeonPathfinder2D.PathCost();
                
                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[layer][b.Position] == CellType.Room) {
                    pathCost.cost += 10;
                } else if (grid[layer][b.Position] == CellType.None) {
                    pathCost.cost += 5;
                } else if (grid[layer][b.Position] == CellType.Hallway) {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null) {
                for (int i = 0; i < path.Count; i++) {
                    var current = path[i];

                    if (grid[layer][current] == CellType.None) {
                        grid[layer][current] = CellType.Hallway;
                    }

                    if (i > 0) {
                        var prev = path[i - 1];
                        if(grid[layer][current] == CellType.Hallway && grid[layer][prev] == CellType.Room){
                            grid[layer][prev] = CellType.RDoor;
                            grid[layer][current] = CellType.HDoor;
                        }
                        var delta = current - prev;
                    }

                    if(i < path.Count - 1){
                        var next = path[i + 1];
                        if(grid[layer][current] == CellType.Hallway && grid[layer][next] == CellType.Room){
                            grid[layer][next] = CellType.RDoor;
                            grid[layer][current] = CellType.HDoor;
                        }
                    }
                }

            }
        }
    }

    void drawFloor(int layer){
        for(int x = 0; x < size.x; x++){
            for(int y = 0; y < size.y; y++){
                var pos = new Vector2Int((int)x, (int)y);
                //floor + ceiling
                if (grid[layer][pos] == CellType.Room) {
                    PlaceRoomz(pos);
                }
                if (grid[layer][pos] == CellType.Hallway) {
                    PlaceHallway(pos);
                }
                if (grid[layer][pos] == CellType.RDoor) {
                    PlaceRDoor(pos);
                }
                if (grid[layer][pos] == CellType.HDoor) {
                    PlaceHDoor(pos);
                    //cek xy+-1 apa ada yang ROOM, kalo ada paksa ubah jadi RDoor, di lantai ngga keliatan keganti tapi
                    if((grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.RDoor) || (grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Room) || (grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.RDoor) || (grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.RDoor)){
                        //d
                    }
                    else{
                        if(grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.Room){
                            grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] = CellType.RDoor;
                        }
                        if(grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Room){
                            grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] = CellType.RDoor;
                        }
                        if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Room){
                            grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] = CellType.RDoor;
                        }
                        if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.Room){
                            grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] = CellType.RDoor;
                        }
                    }
                }
            }
        }
    }

    void drawWall(int layer){
        for(int x = 0; x < size.x; x++){
            for(int y = 0; y < size.y; y++){
                var pos = new Vector2Int((int)x, (int)y);
                //floor + ceiling
                if (grid[layer][pos] == CellType.Room) {
                    cekArch('R', pos, layer);
                }
                if (grid[layer][pos] == CellType.Hallway) {
                    cekArch('H', pos, layer);
                }
                if (grid[layer][pos] == CellType.RDoor) {
                    cekArch('D', pos, layer);
                }
                if (grid[layer][pos] == CellType.HDoor) {
                    cekArch('F', pos, layer);
                }
                if (grid[layer][pos] == CellType.None) {
                    cekArch('N', pos, layer);
                }
            }
        }
    }
        
    void PlaceCube(Vector2Int location, Vector2Int size, Sprite sprite) {
        GameObject go = Instantiate(cubePrefab, new Vector2(location.x * scaling, location.y * scaling), Quaternion.identity, floorP.transform);
        //Sprite sprite = Resources.Load<Sprite>(spritePath);
        go.GetComponent<Transform>().localScale = new Vector2(scaling, scaling);
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        //GameObject roof = Instantiate(cubePrefab, new Vector3(location.x * scaling, scaling, location.y * scaling), Quaternion.identity);
        //roof.GetComponent<Transform>().localScale = new Vector3(2.5f, 0.1f, 2.5f);
        //roof.GetComponent<Transform>().Rotate(180f, 0f, 0f);
        //roof.GetComponent<MeshRenderer>().material = material;
    }

    void cekArch(char gridType, Vector2Int pos, int layer) {
//        var posi = new Vector2Int((int)pos.x, (int)pos.y);
    
        if(gridType == 'R'){
//            posi.x = posi.x + 1;
            //Debug.Log(": " + randSpawn);
            randSpawn--;
            if(randSpawn == 0){
                //player.position = new Vector3(pos.x * scaling, 0f, pos.y  * scaling);
                //[]
                spawnPoint = new Vector2Int((int)pos.x, (int)pos.y);
                //Debug.Log(player.position);
            }
            if(pos.x > 0){
                if(grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Room || grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.RDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                }
                else{
                    //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.Room || grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                }
                else{
                    //placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.Room || grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
            if(pos.y == 0){
                //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
            }
            if(pos.y <= size.y){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Room || grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.RDoor || grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Hallway || grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
        }
        if(gridType == 'H'){
//            posi.x = posi.x + 1;
            if(pos.x > 0){
                if(grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.Hallway || grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.HDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                }
                else{
                    //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.Hallway || grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                }
                else{
                    //placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.Hallway || grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
            if(pos.y == 0){
                //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
            }
            if(pos.y <= size.y){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Hallway || grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.HDoor || grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.Room || grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                }
                else{
                    placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
        }
        if(gridType == 'D'){
//            posi.x = posi.x + 1;
            if(pos.x > 0){
                if(grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.HDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                   //door
                }
                if(grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.None){
                    //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                    //door
                }
                if(grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.None){
                    //placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.None){
                    //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
            if(pos.y == 0){
                //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
            }
            if(pos.y <= size.y){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.HDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.None){
                    placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
        }
        if(gridType == 'F'){    //!cek +-xy, kalo ada merah force ganti jadi ijo!!!![] -> kalo ketemu merah apus semwa wall(wall jadiin 1 child yang sama dulu), terus ulangi generate wall
//            posi.x = posi.x + 1;
            if(pos.x > 0){
                if(grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.RDoor){
                   //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x - 1) + " " + pos.y);
                   //door
                }
                if(grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] == CellType.None){
                    //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.x == 0){
                //placeWallL(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
            }
            if(pos.x <= size.x){
                if(grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + ((int)pos.x + 1) + " " + pos.y);
                    //door
                }
                if(grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] == CellType.None){
                    //placeWallR(pos, new Vector2(0.1f, 1.0f), purpleMaterial);
                }
            }
            if(pos.y > 0){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] == CellType.None){
                    //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
            if(pos.y == 0){
                //placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
            }
            if(pos.y <= size.y){
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.RDoor){
                    //Debug.Log("no wall between " + pos.x + " " + pos.y + " and " + pos.x + " " + ((int)pos.y - 1));
                    //door
                }
                if(grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] == CellType.None){
                    placeWallU(pos, new Vector2(1.0f, 0.1f), wallMaterial);
                }
            }
        }
        if(gridType == 'N'){
            if(pos.x > 0 && grid[layer][new Vector2Int((int)pos.x - 1, (int)pos.y)] != CellType.None){
                placeBorder(pos, new Vector2(0.1f, 1.0f), wallMaterial);
            }
            if(pos.x + 1 < size.x && grid[layer][new Vector2Int((int)pos.x + 1, (int)pos.y)] != CellType.None){
                placeBorder(pos, new Vector2(0.1f, 1.0f), wallMaterial);
            }
            if(pos.y > 0 && grid[layer][new Vector2Int((int)pos.x, (int)pos.y - 1)] != CellType.None){
                placeBorder(pos, new Vector2(0.1f, 1.0f), wallMaterial);
            }
            if(pos.y + 1 < size.y && grid[layer][new Vector2Int((int)pos.x, (int)pos.y + 1)] != CellType.None){
                placeBorder(pos, new Vector2(0.1f, 1.0f), wallMaterial);
            }
        }
    }
    
    void PlaceRoom(Vector2Int location, Vector2Int size) {
        PlaceCube(location, size, floorMaterial);
    }
    void PlaceHallway(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), floorMaterial);
    }
    void PlaceRDoor(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), floorMaterial);
    }
    void PlaceHDoor(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), floorMaterial);
    }
    void PlaceRoomz(Vector2Int location) {
        PlaceCube(location, new Vector2Int(1, 1), floorMaterial);
    }

    void placeWallR(Vector2Int location, Vector2 size, Material material){
        GameObject go = Instantiate(wall, new Vector3((location.x + 0.5f) * scaling, 0, location.y * scaling), Quaternion.identity, wallP.transform);
        go.GetComponent<Transform>().localScale = new Vector3(2.5f, 2.5f, 2.5f);
        go.GetComponent<Transform>().Rotate(0f, -90f, 0f);
        //go.GetComponent<MeshRenderer>().material = material;
    }
    void placeWallL(Vector2Int location, Vector2 size, Material material){
        GameObject go = Instantiate(wall, new Vector3((location.x - 0.5f) * scaling, 0, location.y * scaling), Quaternion.identity, wallP.transform);
        go.GetComponent<Transform>().localScale = new Vector3(2.5f, 2.5f, 2.5f);
        go.GetComponent<Transform>().Rotate(0f, 90f, 0f);
    }
    void placeWallD(Vector2Int location, Vector2 size, Material material){
        GameObject go = Instantiate(wall, new Vector3(location.x * scaling, 0, (location.y + 0.5f) * scaling), Quaternion.identity, wallP.transform);
        go.GetComponent<Transform>().localScale = new Vector3(2.5f, 2.5f, 2.5f);
        go.GetComponent<Transform>().Rotate(0f, 180f, 0f);
    }
    void placeWallU(Vector2Int location, Vector2 size, Sprite material){
        GameObject go = Instantiate(wall, new Vector2(location.x * scaling, (location.y + 0.75f) * scaling), Quaternion.identity, wallP.transform);
        go.GetComponent<Transform>().localScale = new Vector2(scaling, scaling);
    }
    void placeBorder(Vector2Int location, Vector2 size, Sprite material){
        GameObject go = Instantiate(border, new Vector2(location.x * scaling, (location.y) * scaling), Quaternion.identity, wallP.transform);
        go.GetComponent<Transform>().localScale = new Vector2(scaling, scaling);
        //go.GetComponent<Transform>().localScale = new Vector3(2.5f, 2.5f, 2.5f);
    }

    void changeLayer(int layer){
        for(int i = 0; i < 10; i++){
            if(i == layer){
                Layer[i].SetActive(true);
            }
            else{
                Layer[i].SetActive(false);
            }
        }
    }

    /*
    void generateEnemy(int count){
        for(int i = count; i > 0; i--){
            //pick random location
            tar = new Vector2Int(Random.Range(0,Generator2D.size.x), Random.Range(0,Generator2D.size.y));
            //check if not null
            if(Generator2D.grid[layer][tar] != CellType.None){
                Debug.Log("spawn enemy at " + Generator2D.grid[layer][tar]);
                GameObject enem = Instantiate(enemy, new Vector3((tar.x + 0.5f) * scaling, 0.5f, (tar.y + 0.5f) * scaling), Quaternion.identity);
                enem.GetComponent<Transform>().localScale = new Vector3(1.5f, 1.5f, 1.5f); 
                enem.GetComponent<Transform>().Rotate(0f, Random.Range(0f,360f), 0f);
                //enem.GetComponent<MeshRenderer>().material = purpleMaterial;
            }
            else{
                //if null repeat loop 
                i++;
            }
            //spawn enemy, count -= 1
            //repeat until count = 0;
        }
    }*/
}
