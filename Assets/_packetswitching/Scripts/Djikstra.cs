﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Djikstra {

	public Node source;
	public Dictionary<Node, Node> prev;
	public Dictionary<Node, float> dist;
	public Djikstra(Node source, Dictionary<Node, float> dist, Dictionary<Node, Node> prev) {
		this.source = source; this.prev = prev; this.dist = dist;
	}

	public static Node MinDist(List<Node> Q, Dictionary<Node, float> dist) {
		float min = float.PositiveInfinity;
		Node best = null;
		for (int i = 0; i < Q.Count; ++i) {
			Node key = Q [i];
			float value = dist [key];
			if (best == null || value < min) {
				min = value;
				best = key;
			}
		}
		return best;
	}

	public static Djikstra Create(List<Node> Graph, Node source){
		Djikstra d = new Djikstra(source, new Dictionary<Node, float> (), new Dictionary<Node, Node> ());
		d.Calculate (Graph, source);
		return d;
	}

	// original algorithm: https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
	// 1function Dijkstra(Graph, source):
	public void Calculate(List<Node> Graph, Node source) {
		// 2
		if(source != null) {
			// 3 create vertex set Q
			List<Node> Q = new List<Node>();
			// 5 for each vertex v in Graph:             // Initialization
			for(int i = 0; i < Graph.Count; ++i){
				Node v = Graph [i];
				// 6 dist[v] ← INFINITY                  // Unknown distance from source to v
				dist[v] = float.PositiveInfinity;
				// 7 prev[v] ← UNDEFINED                 // Previous node in optimal path from source
				prev[v] = null;
				// 8 add v to Q                          // All nodes initially in Q (unvisited nodes)
				Q.Add(v);
			}
			//10 dist[source] ← 0                        // Distance from source to source
			dist[source] = 0;
			int iter = 0;
			//12 while Q is not empty:
			while (Q.Count > 0 && iter++ < 100) {
				//13 u ← vertex in Q with min dist[u]    // Node with the least distance will be selected first
				Node u = MinDist(Q, dist);
				if (u == null) {
					Debug.LogError ("how on earth did a null get into the distance dictionary?");
				}
				//14 remove u from Q 
				if (!Q.Remove (u)) {
					Debug.Log ("woah, woah, woah.");
				}
				// Debug.Log ("("+u+") "+Q.Count);

				//16 for each neighbor v of u:           // where v is still in Q.
				for(int i = 0; i < u.edges.Count; ++i) {
					Edge edge = u.edges [i];
					Node v = edge.Other(u);
					if (v == null) {
						Debug.LogError ("really? edge to knowhere? "+u.name+"'s edge "+i+" is bad.");
					}
					if (!edge.Has (u, v)) {
						Debug.LogError ("BAD EDGE : "+u.name+" and "+v.name+" not both in edge "+edge);
					}
					//17 alt ← dist[u] + length(u, v)
					float alt = dist[u] + edge.totalDistance + u.GetTravelCost();
					//18 if alt < dist[v]:               // A shorter path to v has been found
					if(alt < dist[v]) {
						//19 dist[v] ← alt 
						dist[v] = alt;
						//20 prev[v] ← u 
						prev[v] = u;
					}
				}
			}
		}
		//22 return dist[], prev[]
	}

	public List<Node> Path(Node target) {
		//1 S ← empty sequence
		List<Node> S = new List<Node>();
		//2 u ← target
		Node u = target;
		int iter = 0;
		//3 while prev[u] is defined:                  // Construct the shortest path with a stack S
		while(prev[u] != null && iter++ < 1000) {
			//4     insert u at the beginning of S         // Push the vertex onto the stack
			S.Add(u);
			//5     u ← prev[u]                            // Traverse from target to source
			u = prev[u];
		}
		//6 insert u at the beginning of S             // Push the source onto the stack
		S.Add(u);
		S.Reverse ();
		return S;
	}

	public Vector3[] PathAsPoints(Node target){
		List<Node> path = Path (target);
		Vector3[] result = new Vector3[path.Count];
		for (int i = 0; i < path.Count; ++i) {
			result [i] = path [i].transform.position;
		}
		return result;
	}

	public bool FastestPathForNodeLeadsThroughListItem(Node node, List<Node> updating){
		while (node != source && node != null) {
			if (updating.IndexOf (node) >= 0) {
				return true;
			}
			if (!prev.TryGetValue (node, out node)) {
				node = null; // if this node is not connected to the graph that source is considered the root of
			}
		}
		return false;
	}

	public bool IsEdgeSignificant(Edge e){ return IsEdgeSignificant (e.a, e.b); }
	public bool IsEdgeSignificant(Node a_from, Node a_to) {
		Node l_from, l_to;
		if (prev.TryGetValue (a_from, out l_from) && prev.TryGetValue (a_to, out l_to)) {
			return l_from == a_to || l_to == a_from;
		}
		return false;
	}

	public void Update(List<Node> toUpdate, List<Node> allNodes) {
		HashSet<Node> set = new HashSet<Node> ();
		for (int i = 0; i < toUpdate.Count; ++i) {
			set.Add (toUpdate [i]);
		}
		for (int i = 0; i < allNodes.Count; ++i) {
			if (FastestPathForNodeLeadsThroughListItem (allNodes [i], toUpdate)) {
				set.Add (allNodes [i]);
			}
		}
		toUpdate.Clear ();
		toUpdate.AddRange (set);
		Calculate (toUpdate, source);
	}

	public void CalcLines(ref List<GameObject> pathlist) {
		// get all leaves
		List<Node> allNodes = new List<Node>();
		foreach (KeyValuePair<Node, Node> p in prev) {
			allNodes.Add (p.Key);
		}
		List<Node> leafNodes = new List<Node> ();
		leafNodes.AddRange (allNodes);
		foreach (KeyValuePair<Node, Node> p in prev) {
			leafNodes.Remove (p.Value);
		}
		if (pathlist == null) {
			pathlist = new List<GameObject> ();
		}
		if (pathlist.Count < leafNodes.Count) {
			for (int i = pathlist.Count; i < leafNodes.Count; ++i) {
				GameObject go = new GameObject ("path " + i);
				pathlist.Add (go);
			}
		}
		// draw lines to leaves.
		for (int i = 0; i < leafNodes.Count; ++i) {
			Vector3[] path = PathAsPoints (leafNodes [i]);
			GameObject go = pathlist [i];
			LineRenderer lr = Lines.Make (ref go, path, path.Length, Color.white, 0, 1);
			lr.startColor = Color.cyan;
			lr.endColor = Color.red;
			lr.numCapVertices = 4;
			lr.numCornerVertices = 4;
			go.SetActive (true);
			pathlist [i] = go;
		}
		for (int i = leafNodes.Count; i < pathlist.Count; ++i) {
			pathlist [i].SetActive (false);
		}
	}

	public void DebugPrint() {
		string txt = "list for "+source.name+"\n";
		foreach (KeyValuePair<Node, float> d in dist) {
			string n = "?";
			if (prev [d.Key] != null) {
				n = prev [d.Key].name;
			}
			txt += "to " + d.Key.name + " (" + d.Value + ") from " + n + "\n";
		}
		Debug.Log (txt);
	}
}
