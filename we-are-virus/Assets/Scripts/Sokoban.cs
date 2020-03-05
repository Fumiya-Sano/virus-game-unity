using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sokoban : MonoBehaviour
{
    // タイルの種類
    private enum TileType
    {
        NONE,  // 何もない
        GROUND,  //  地面
        VIRUS_1,
        VIRUS_2,
        VIRUS_3,
        VIRUS_4,
    }

    // 方向の種類
    private enum DirectionType
    {
        UP,  // 上
        RIGHT,  //　右
        DOWN, 　// 下
        LEFT,  // 左
    }

    private int playerNum = 4;  // プレイヤーの人数
    public TextAsset stageFile;  // ステージ構造が記述されたテキストファイル
    
    private int rows; // 行数
    private int columns;  // 列数
    private TileType[,] tileList;  // タイル情報を管理する二次元配列

    public float tileSize;  // タイルのサイズ

    public Sprite groundSprite;  // 地面のスプライト
    public Sprite[] playerSprite;  // プレイヤー1~8のスプライト配列
    public Sprite[] virusSprite;  // ウイルスに感染した地面のスプライト配列

    private GameObject[] playerlist = new GameObject [8];  // プレイヤーのゲームオブジェクト
    private Vector2 middleOffset;  // 中心位置


    // 各位置に存在するゲームオブジェクトを管理する連想配列
    
    private Dictionary<GameObject, Vector2Int> virusPosTable = new Dictionary<GameObject, Vector2Int>();

    // ゲームプレイヤーの位置を管理する連想配列
    private Dictionary<GameObject, Vector2Int> playerPosTable = new Dictionary<GameObject, Vector2Int>();




    // Start is called before the first frame update
    void Start()
    {
        LoadTileData();
        CreateStage();
        CreatePlayerList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // タイル情報を読み込む
    private void LoadTileData()
    {
        // タイル情報を1行ごとに分割
        var lines = stageFile.text.Split
        (
            new[] { '\r',  '\n'},  // \rか\nで区切って、配列化
            System.StringSplitOptions.RemoveEmptyEntries  // 要素がないところがなくす
        );

        // 1行目を、,で区切って配列化
        var nums = lines[0].Split(new[] {','});

        // タイルの列数と行数を保持
        rows = lines.Length;  // 行数
        columns = nums.Length;  // 列数

        // タイル情報をint型の２次元配列で保持
        tileList = new TileType[ columns, rows ];
        for (int y = 0; y < rows; y++)
        {
            // 1文字ずつ取得
            var st = lines[y];
            nums = st.Split(new[] { ',' });
            for (int x = 0; x < columns; x++)
            {
                // 読み込んだ文字を数値に変換して保持
                tileList[x, y] = ( TileType )int.Parse(nums[x]);
            }
        }  
    }

    // プレイヤーリストの作成と配置
    private void CreatePlayerList()
    {
        for(int i = 1; i <= playerNum; i++)
        {
            var name = "player" + i;

            playerlist[i - 1] = new GameObject(name);

            var sr = playerlist[i - 1].AddComponent<SpriteRenderer>();

            sr.sprite = playerSprite[i - 1];

            sr.sortingOrder = 2;

            int x = Random.Range(0, columns);
            int y = Random.Range(0, rows);

            playerlist[i - 1].transform.position = GetDisplayPosition(x, y);

            playerPosTable.Add(playerlist[i-1], new Vector2Int(x, y));
        }
    }

    // ステージ作成
    private void CreateStage()
    {
        // ステージの中心位置を計算
        middleOffset.x = columns * tileSize * 0.5f - tileSize * 0.5f;
        middleOffset.y = rows * tileSize * 0.5f - tileSize * 0.5f;

        for(int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                var val = tileList[x, y];

                // 何もない場所は無視
                if(val == TileType.NONE) continue;

                // タイルの名前に行番号と列番号を付与
                var name = "tile" + y + "_" + x;

                // タイルのゲームオブジェクトを作成
                var tile = new GameObject(name);

                // タイルにスプライトを描画する機能を追加
                var sr = tile.AddComponent<SpriteRenderer>();

                // タイルのスプライトを設定
                sr.sprite = groundSprite;

                // タイルの位置を設定
                tile.transform.position = GetDisplayPosition(x, y);
            }
        }
    }

    // 指定された行番号と列番号からスプライトの表示位置を計算して返す
    private Vector2 GetDisplayPosition(int x, int y)
    {
        return new Vector2
        (
        x * tileSize - middleOffset.x,
        y * -tileSize + middleOffset.y + 3
        );
    }
    // 指定された位置に存在するウイルスを返します
    private GameObject GetVirusAtPosition(Vector2Int pos)
    {
        foreach (var pair in virusPosTable)
        {
            // 指定された位置が見つかった場合
            if(pair.Value == pos)
            {
                // その位置の存在するゲームオブジェクトを返す
                return pair.Key;
            }
        }
        return null;
    }

    // 指定された位置に存在するプレイヤーを返します
    private GameObject GetPlayerAtPosition(Vector2Int pos)
    {
        foreach(var pair in playerPosTable)
        {
            // 指定された位置が見つかった場合
            if(pair.Value == pos)
            {
                // その位置の存在するゲームオブジェクトを返す
                return pair.Key;
            }
        }
        return null;
    }

    // 指定された位置がステージ内でかつNONE以外ならtrueを返す
    private bool IsValidPosition(Vector2Int pos)
    {
        if(0 <= pos.x && pos.x < columns && 0 <= pos.y && pos.y < rows)
        {
            return tileList[pos.x, pos.y] != TileType.NONE;
        }
        return false;
    }
    /*
    // 指定された位置のタイルがウイルスに感染しているならtrueを返す
    private bool IsVirus(Vector2Int pos)
    {
        var cell = tileList[pos.x, pos.y];
        return (cell == TileType.VIRUS_1) || (cell == TileType.VIRUS_2) || (cell == TileType.VIRUS_3) || (cell == TileType.VIRUS_4);
    }
    */

    // 移動処理とその回数を管理する関数
    private void Walk(GameObject player)
    {
        // 上矢印が押された場合
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // プレイヤーが上に移動できるか検証
            TryMovePlayer(DirectionType.UP, player);
        }
        // 右矢印が押された場合
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // プレイヤーが右に移動できるか検証
            TryMovePlayer(DirectionType.RIGHT, player);
        }
        // 下矢印が押された場合
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // プレイヤーが下に移動できるか検証
            TryMovePlayer(DirectionType.DOWN, player);
        }
        // 左矢印が押された場合
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // プレイヤーが左に移動できるか検証
            TryMovePlayer(DirectionType.LEFT, player);
        }

    }

    // 指定された方向にプレイヤーが移動できるか検証
    // 移動できる場合は移動する
    private void TryMovePlayer(DirectionType direction, GameObject player)
    {
        // プレイヤーの現在地を取得
        var currentPlayerPos = playerPosTable[player];  // 任意tのプレイヤーの現在地を取得

        // プレイヤーの移動先の位置を計算
        var nextPlayerPos = GetNextPositionAlong(currentPlayerPos, direction);

        // プレイヤーの移動先がステージ内ではない場合は無視
        if(!IsValidPosition(nextPlayerPos)) return;

        // プレイヤーの移動
        player.transform.position = GetDisplayPosition(nextPlayerPos.x, nextPlayerPos.y);

        // プレイヤーの位置を更新
        playerPosTable[player] = nextPlayerPos;
    }

    // 指定された方向の位置を返す
    private Vector2Int GetNextPositionAlong(Vector2Int pos, DirectionType direction)
    {
        switch(direction)
        {
            // 上
            case DirectionType.UP:
                pos.y -= 1;
                break;
            
            // 右
            case DirectionType.RIGHT:
                pos.x += 1;
                break;
            
            //　下
            case DirectionType.DOWN:
                pos.y += 1;
                break;

            // 左
            case DirectionType.LEFT:
                pos.x -= 1;
                break;
        }
        return pos;
    }

}
