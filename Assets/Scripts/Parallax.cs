using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    // keep track all of the object types: Pipes, Clouds, and Stars
    class PoolObject {
        public Transform transform;
        public bool inUse; // determine if it is available or not

        // Constructor initialization
        public PoolObject (Transform t) {
            transform = t;
        }

        public void Use() { inUse = true; }

        public void Dispose() { inUse = false; }
    }

    [System.Serializable]
    public struct YSpawnRange {
        public float min;
        public float max;
    }

    public GameObject Prefab;
    public int poolSize; // how many pipes, how many clouds
    public float shiftSpeed;
    public float spawnRate;

    public YSpawnRange yspawnrange;
    public Vector3 defaultSpawnPos;

    // particle such as clouds and stars
    public bool spawnImmediate; // except pipes

    public Vector3 immediateSpawnPosition;
    public Vector2 targetAspectRatio; // Aspect Ratios, needs to be used for all platform

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjectsArr;
    GameManager game;

    void Awake() {
        Configure();
    }

    void Start() {
        game = GameManager.GameInstance;
    }

    void OnEnable(){
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable(){
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed() {
        // reset all
        for (int i = 0; i < poolObjectsArr.Length; i++){
            poolObjectsArr[i].Dispose();
            poolObjectsArr[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    void Update() {
        if (game.GameOver) return;

        Shift();
        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate){
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure() {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y; // 10:16
        poolObjectsArr = new PoolObject[poolSize];
        for (int i = 0; i < poolObjectsArr.Length; i++){
            GameObject obj = Instantiate(Prefab);
            Transform t = obj.transform;

            t.SetParent(transform);
            t.position = Vector3.one * 1000;

            poolObjectsArr[i] = new PoolObject(t);
        }

        if (spawnImmediate) {
            SpawnImmediate();
        }
    }

    void Spawn() {
        Transform t = GetPoolObject();
        if (t == null) return; // if true, this indicates that poolsize is too small

        // Position that parallax will bespawned
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(yspawnrange.min, yspawnrange.max);
        t.position = pos;
    }

    void SpawnImmediate() {
        Transform t = GetPoolObject();
        if (t == null) return; // if true, this indicates that poolsize is too small

        // Position that parallax will bespawned
        Vector3 pos = Vector3.zero;
        pos.x = (defaultSpawnPos.x * Camera.main.aspect) / targetAspect;
        pos.y = Random.Range(yspawnrange.min, yspawnrange.max);
        t.position = pos;

        Spawn();
    }

    void Shift() {
        for (int i = 0; i < poolObjectsArr.Length; i++ ){
            poolObjectsArr[i].transform.localPosition += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjectsArr[i]);
        }

    }

    // if the object can be dispoed
    // check if the object is still in use (on the screen) or not
    void CheckDisposeObject(PoolObject poolObject){
        if (poolObject.transform.position.x < (-defaultSpawnPos.x * Camera.main.aspect) / targetAspect) {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject(){
        for (int i = 0; i < poolObjectsArr.Length; i++){
            if(!poolObjectsArr[i].inUse){
                poolObjectsArr[i].Use();
                return poolObjectsArr[i].transform;
            }
        }
        return null;
    }
}
