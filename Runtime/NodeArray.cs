using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Diagnostics;
using UnityEngine;
//using Debug = UnityEngine.Debug;

[ExecuteInEditMode] 
public class NodeArray : MonoBehaviour
{

    public Vector3 arraySize = Vector3.one;

    public float nodeSpacing; //Ammount of Nodes per Unity space Unit

    [SerializeField]
    LayerMask groundLayer = -1;

    [HideInInspector]
    public List<Vector3> allNodes = new List<Vector3>();

    private Dictionary<Vector2Int, Node> _NODES;

    public bool showNodes;

    private bool _generatingNeighbours;

    public static NodeArray Ins { get; private set; }

    //Grid System
    private Vector3 gridStartPoint;
    private float gridSize = 10;

    private static readonly Vector2Int[] dirChecks = new Vector2Int[4] {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.right,
        Vector2Int.left,
    };

    private void Awake()
    {
        if (Ins == null)
            Ins = this;
        GenerateNodes();
    }

    //public void GenerateNodes() => StartCoroutine(GenerateNodesCor());

    public void GenerateNodes()
    {
        //watch = new Stopwatch();
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();

        int ammountX = Mathf.RoundToInt((arraySize.x) / nodeSpacing);
        int ammountZ = Mathf.RoundToInt((arraySize.z) / nodeSpacing);

        gridStartPoint = transform.position - arraySize / 2;
        //DrawCross(startPoint, 50f, Color.green, 20);

        float YCoord = transform.position.y + arraySize.y/2;

        _NODES = new Dictionary<Vector2Int, Node>();

        int counter = 0;
        //watch.Start();

        for (int x = 0; x < ammountX; x++) {
            for (int z = 0; z < ammountZ; z++) {
                counter++;
                float _X = transform.position.x - (arraySize.x / 2) + (x * nodeSpacing);
                float _Z = transform.position.z - (arraySize.z / 2) + (z * nodeSpacing);

                Vector3 start = new Vector3(_X, YCoord, _Z);

                if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, arraySize.y, groundLayer)) {
                    if (!hit.collider.gameObject.CompareTag("Obstacle") && !hit.collider.gameObject.CompareTag("Actor")) {

                        var objs = Physics.OverlapSphere(hit.point, nodeSpacing * 0.6f);

                        if (!objs.Any(l => l.gameObject.CompareTag("Obstacle"))) {
                            _NODES.Add(new Vector2Int(x, z), new Node {
                                pos = hit.point,
                                normal = hit.normal,
                            });
                        }

                    }

                }

                //if (watch.ElapsedMilliseconds > 60) {
                //    watch.Restart();
                //    yield return null;
                //}

            } 
        }

        //watch.Stop();
        //print($"Corroutine Finished - {_NODES.Count} Nodes Generated");
        //StartCoroutine(GenerateNeighbours());
    }
    /// <summary>
    /// Does not work currently, requires modifing some code
    /// Better to Cache neighbours when asked
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateNeighbours()
    {
        int counter = 0;
        _generatingNeighbours = true;
        foreach (var node in _NODES) {
            counter++;
            List<Vector2Int> ns = new List<Vector2Int>();
            for (int i = 0; i < dirChecks.Length; i++) {
                var index = node.Key + dirChecks[i];
                if (_NODES.ContainsKey(index) && Vector3.Distance(node.Value.pos, _NODES[index].pos) <= nodeSpacing * 1.5f) {
                    ns.Add(index);
                }
            }
            _NODES[node.Key] = new Node { 
                pos = node.Value.pos,
                //neighbours = ns.ToArray(),
            };
            if (counter % 100 == 0)
                yield return null;
        }
        _generatingNeighbours = false;
    }
    public List<Vector3> GetNeighbours(Vector3 pos)
    {
        List<Vector3> output = new List<Vector3>();
        if (!FindClosest(pos, out Vector2Int location)) {
            //No node is Close Enough
            return output;
        }

        if (_NODES.ContainsKey(location)) {
            for (int i = 0; i < 4; i++) {
                if (_NODES.ContainsKey(location + dirChecks[i]) && Vector3.Distance(_NODES[location + dirChecks[i]].pos, _NODES[location].pos) <= nodeSpacing * 1.1f) {
                    output.Add(_NODES[location + dirChecks[i]].pos);
                }
            }
        }
        return output;
    }
    public bool IsNeighbour(Vector2Int a, Vector2Int b)
    {
        if (_NODES.ContainsKey(a) && _NODES.ContainsKey(b)) {
            for (int i = 0; i < 4; i++) {
                if (a + dirChecks[i] == b && Vector3.Distance(_NODES[a].pos, _NODES[b].pos) <= nodeSpacing * 1.1f) {
                    return true;
                }
            }
        }
        return false;
    }

    public bool FindClosest(Vector3 pos, out Vector2Int closest)
    {
        Vector3 relativePos = pos - gridStartPoint;

        int X = Mathf.RoundToInt(relativePos.x / nodeSpacing);
        int Z = Mathf.RoundToInt(relativePos.z / nodeSpacing);

        closest = new Vector2Int(X, Z);

        if (!_NODES.ContainsKey(closest))
        {
            //Check Sorrounding Places
            bool found = false;
            float minDistance = float.MaxValue;
            for (int i = 0; i < PathDebug.dirChecks.Length; i++)
            {
                var newClosest = closest + PathDebug.dirChecks[i];

                if (!_NODES.ContainsKey(newClosest)) {
                    continue;
                }

                var newDist = Vector3.Distance(pos, _NODES[newClosest].pos);
                if (newDist < minDistance)
                {
                    found = true;
                    minDistance = newDist;
                }

            }

            return found;
        }
        Vector3 nodedPos = pos;
        nodedPos.y = _NODES.FirstOrDefault().Value.pos.y;
        float distance = Vector3.Distance(nodedPos, _NODES[closest].pos);

        if (distance < nodeSpacing * 0.9f)
            return true;
        return false;
    }
    public bool GetPos(Vector2Int key, out Vector3 pos)
    {
        if (_NODES.ContainsKey(key))
        {
            //print("Contains Key?");
            pos = _NODES[key].pos;
            return true;
        }

        pos = default;
        return false;
    }
    public bool IsWithinBounds(Vector3 pos) {
        if (
            //Find if any value is Greater
            transform.position.x + pos.x > transform.position.x + arraySize.x ||
            transform.position.y + pos.y > transform.position.y + arraySize.y ||
            transform.position.z + pos.z > transform.position.z + arraySize.z ||
            //Find if any value is Lower                          
            transform.position.x - pos.x < transform.position.x - arraySize.x ||
            transform.position.y - pos.y < transform.position.y - arraySize.y ||
            transform.position.z - pos.z < transform.position.z - arraySize.z
            ) {
            return false;
        }

        return true;
    }
    public bool CheckSight(Vector2Int nodeA, Vector2Int nodeB)
    {
        if (!_NODES.ContainsKey(nodeA) || !_NODES.ContainsKey(nodeB)) {
            return false;
        }
        float stepX = nodeB.x - nodeA.x;
        float stepY = nodeB.y - nodeA.y;
        Vector2Int current = nodeA;
        Vector2 currentPos = nodeA;
        bool checkY = false;
        int count = 0;
        while(true) {
            count++;
            if (count > 500)
                return false;
            if (!checkY) { //Check X
                currentPos += new Vector2(stepX, 0);
            }
            else { //Check Y
                currentPos += new Vector2(0, stepY);
            }
            var next = Vector2Int.RoundToInt(currentPos);
            if (!IsNeighbour(current, next)) {
                return false;
            }
            current = next;
            checkY = !checkY;
        }
    }
    public bool IsBorder(Vector2Int node)
    {
        for (int i = 0; i < PathDebug.dirChecks.Length; i++)
        {
            var current = node + PathDebug.dirChecks[i];

            if (!_NODES.ContainsKey(current))
            {
                return true;
            }
            var extraindex = i + 1;
            if (extraindex >= PathDebug.dirChecks.Length)
                extraindex = 0;

            if (!_NODES.ContainsKey(current + PathDebug.dirChecks[extraindex]))
            {
                return true;
            }

            


        }
        var path = PathfindBFS<Vector2Int>.Run(
                node,
                n => !_NODES.ContainsKey(n),
                x =>
                {
                    List<Vector2Int> ret = new List<Vector2Int>();
                    for (int i = 0; i < PathDebug.dirChecks.Length; i++)
                    {
                        ret.Add(node + PathDebug.dirChecks[i]);
                    }
                    return ret;
                },
                16
                );
        if (path.Count > 0 && Vector2Int.Distance(path.Last(), node) < nodeSpacing * 2)
            return true;


        return false;
    }
    public bool IsBorder(Vector3 exact)
    {
        if (!FindClosest(exact, out var close))
        {
            return true;
        }

        return IsBorder(close);


    }
    private void OnValidate() {
        if (arraySize.x < 0)
            arraySize.x = 0;
        if (arraySize.y < 0)
            arraySize.y = 0;
        if (arraySize.z < 0)
            arraySize.z = 0;
        if (nodeSpacing < 0.1f)
            nodeSpacing = 0.1f;
    }
    private void OnDrawGizmosSelected()
    {
        if (_generatingNeighbours)
            return;
        //Debug.Log("EXECUTE");
        //Draw the node Area
        Gizmos.DrawWireCube(transform.position, arraySize);

        if (showNodes && _NODES != null) {

            var cam =
#if UNITY_EDITOR
                UnityEditor.SceneView.lastActiveSceneView.camera;
#else
                Camera.main;
#endif


            Gizmos.color = Color.red;
            int limit = 4000;
            foreach (var node in _NODES.Values) {
                if (limit <= 0)
                    break;

                Vector3 cp = cam.WorldToViewportPoint(node.pos);
                if (cp.x < 0 || cp.x > 1 || cp.y < 0 || cp.y > 1)
                    continue;

                Gizmos.DrawSphere(node.pos, .1f);
                limit--;
            }
        }

    }

    public static void DrawCross(Vector3 pos, float size, Color color, float duration)
    {
        size /= 2;
        Debug.DrawLine(pos + Vector3.right   * size, pos + Vector3.left * size  , color, duration);
        Debug.DrawLine(pos + Vector3.up      * size, pos + Vector3.down * size  , color, duration);
        Debug.DrawLine(pos + Vector3.forward * size, pos + Vector3.back * size  , color, duration);
    }


    private struct Node
    {
        public Vector3 pos;
        public Vector3 normal;
    }

}
