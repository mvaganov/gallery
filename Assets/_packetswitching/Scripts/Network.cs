using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Network : MonoBehaviour {

	public Node prefabNode;
	public List<Node> nodes = new List<Node>();
	public Packet prefabPacket;
	public TMPro.TextMeshPro prefabNodelabel;
	public int packetsPerNode = 1;
	public int randomPackets = 50;
	int randomPacketsCreated = 0;
	public float generationRadius = 2;
	public float enforceBoundary = 4;
	public Dictionary<Node, Djikstra> navdata = new Dictionary<Node, Djikstra>();
	public string showPathsFor;
	private List<GameObject> pathLines;

	[System.Serializable]
	public class NodeSetting {
		public string name;
		public Node.Settings settings;
		public int nodeCount = 20;
		[HideInInspector]
		public int nodesCreated = 0;
		public NodeSetting(string name, Node.Settings settings){this.name=name;this.settings=settings;}
	}
	public List<NodeSetting> nodeSettings = new List<NodeSetting>(){
		new NodeSetting("default", new Node.Settings())
	};

	public Node GetNodeLabeled(string labeltext){
		for (int i = 0; i < nodes.Count; ++i) {
			if (nodes [i].GetLabel () == labeltext) {
				return nodes [i];
			}
		}
		return null;
	}

	public void UpdateDjikstraDebug(){
		Node nodeToDraw = GetNodeLabeled(showPathsFor);
		if(nodeToDraw != null) {
			navdata [nodeToDraw].CalcLines (ref pathLines);
		}
	}

	// TODO Djikstra's table for path-finding

	public void UpdateDjikstra() {
		for (int i = 0; i < nodes.Count; ++i) {
			Djikstra dd;
			Node n = nodes [i];
			if (navdata.TryGetValue (n, out dd)) {
				dd.Calculate (nodes, n);
			} else {
				navdata [n] = Djikstra.Create (nodes, n);
			}
//			navdata [n].DebugPrint ();
		}
		UpdateDjikstraDebug ();
	}

	// node cost increase
	// node cost decrease
	// edge cost increase
	public void UpdateEdgeCostIncrease(Node a_from, Node a_to) {
		// recalculate all the nodes with paths that travel across the nodes in this edge
	}
	// edge cost decrease
	public void UpdateEdgeCostDecrease(Node a_from, Node a_to) {
		// recalculate all the nodes with this edge, then recalculate all nodes with paths that travel across those nodes
	}
	// edge forms
	public void UpdateEdgeForm(Node a_from, Node a_to) {
		UpdateEdgeCostDecrease (a_from, a_to);
	}
	// edge breaks
	public void UpdateEdgeBreak(Node a_from, Node a_to) {
		List<Node> updated = new List<Node> ();
		updated.Add (a_from);
		updated.Add (a_to);

		Djikstra d;
		// updating the nav edge
		foreach (KeyValuePair<Node, Djikstra> entry in navdata) {
			if (entry.Value.IsEdgeSignificant(a_from, a_to)) {
				entry.Value.Update (updated, nodes);
			}
		}
//		for (int i = 0; i < updated.Count; ++i) {
//			Node n = updated [i];
//			if (!navdata.TryGetValue (n, out d)) {
//				navdata [n] = Djikstra.Create (nodes, n);
//			}
//		}
		UpdateDjikstraDebug();
	}

	// Use this for initialization
	BoxCollider bc;

	void GenerateNext(NodeSetting ns) {
		Vector3 p;
		Vector3 start;
		int interations = 0;
		do {
			if (transform.childCount == 0) {
				start = transform.position;
			} else {
				start = transform.GetChild (Random.Range (0, transform.childCount)).transform.position;
			}
			p = Random.onUnitSphere * generationRadius + start;
			interations++;
		} while(!bc.bounds.Contains (p) && interations < 100);
		GameObject node = Instantiate(prefabNode.gameObject, p, Quaternion.identity);
		node.transform.SetParent (transform);
		Node sn = node.GetComponent<Node>();
		sn.settings = ns.settings;
		sn.name = ((char)('A' + nodes.Count)).ToString ();
		TMPro.TextMeshPro tmp = (Instantiate (prefabNodelabel)).GetComponent<TMPro.TextMeshPro> ();
		tmp.transform.SetParent (sn.transform);
		tmp.transform.localPosition = Vector3.zero;
		tmp.transform.localRotation = Quaternion.identity;
		tmp.text = sn.name;
		nodes.Add (sn);
		sn.network = this;
		for(int i = 0; i < packetsPerNode; ++i){
			CreatePacket (sn);
		}
		ns.nodesCreated++;
	}

	public Node GetRandomNode() {
		return nodes[Random.Range (0, nodes.Count)];
	}

	public Packet CreatePacket(Node serverNode) {
		Packet packet = (Instantiate(prefabPacket.gameObject) as GameObject).GetComponent<Packet>();
		packet.current = serverNode;
		packet.transform.position = serverNode.transform.position + Random.onUnitSphere;
		return packet;
	}

	void Start () {
		bc = GetComponent<BoxCollider>();
		StartCoroutine(Calcit(4f));
	}
	IEnumerator Calcit(float waitTime) {
		yield return new WaitForSeconds(waitTime);
		UpdateDjikstra ();
	}

	// Update is called once per frame
	void FixedUpdate () {
		for (int s = 0; s < nodeSettings.Count; ++s) {
			NodeSetting ns = nodeSettings [s];
			if (ns.nodesCreated < ns.nodeCount) {
				GenerateNext (ns);
			}
		}
		if(randomPacketsCreated < randomPackets) {
			CreatePacket (GetRandomNode());
			randomPacketsCreated++;
		}
		if (enforceBoundary > 0) {
			for (int i = 0; i < transform.childCount; ++i) {
				if (!bc.bounds.Contains (transform.GetChild (i).position)) {
					Rigidbody rb = transform.GetChild (i).GetComponent<Rigidbody> ();
					if (rb) {
						Vector3 delta = transform.position - rb.transform.position;
						rb.velocity += delta.normalized * enforceBoundary;
					}
				}
			}
		}
	}
}
