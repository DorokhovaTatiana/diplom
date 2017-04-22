/*
 * Created by SharpDevelop.
 * User: Татьяна
 * Date: 04.04.2017
 * Time: 21:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using QuickGraph.Algorithms;

namespace diplom_v1
{
	/// <summary>
	/// Description of NodeSequence.
	/// </summary>
	[Serializable]
	public class NodeSequence
	{
		public class FileDotEngine : IDotEngine
        {    
            public string Run(GraphvizImageType imageType, string dot, string outputFileName)
            {
                string output = outputFileName;
                if (!output.EndsWith(".dot", StringComparison.InvariantCultureIgnoreCase))
                    output = output + ".dot";
    
                File.WriteAllText(output, dot);
                ProcessStartInfo startInfo = new ProcessStartInfo("dot.exe");
                var nameImage  = output.Remove(output.IndexOf('.') + 1) + "png";
                startInfo.Arguments = string.Format(@"-Tpng {0} -o {1}", output, nameImage);
                Process.Start(startInfo);
                return output;
            }
        }
		
		List<NodeSequence> children { get; set; }
		public Dictionary<Tuple<int, int>, NodeSequence> parents { get; set; }
		List<int> sequence { get; set; }
		public string name { get; set; }
		List<string> used = new List<string>();
		public int[,] matrix { get; set; }
		public List<NodeSequence> maximumGraphicsSequence = new List<NodeSequence>();
		public int valueIncrease { get; set; }
		public int valueReduction { get; set; }
//		Dictionary <NodeSequence, Tuple<int, int>> dict { get; set; }
		public List <AdjacencyGraph<int, Edge<int>>> graphs { get; set; }
		

		public NodeSequence(List<int> sequence)
		{
			this.name = String.Join(",", sequence);
			this.sequence = sequence;
			this.children = new List<NodeSequence>();
			this.parents = new Dictionary<Tuple<int, int>, NodeSequence>();
			this.matrix = null;
//			this.dict = new Dictionary<NodeSequence, Tuple<int, int>>();
			this.graphs = new List <AdjacencyGraph<int, Edge<int>>>();
		}
		
		public T Clone<T>(T source)
	    {
	        if (!typeof(T).IsSerializable)
	        {
	            throw new ArgumentException("The type must be serializable.", "source");
	        }
	
	        if (Object.ReferenceEquals(source, null))
	        {
	            return default(T);
	        }
	
	        System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
	        Stream stream = new MemoryStream();
	        using (stream)
	        {
	            formatter.Serialize(stream, source);
	            stream.Seek(0, SeekOrigin.Begin);
	            return (T)formatter.Deserialize(stream);
	        }
	    }
		
		public override string ToString(){
			var answer = "";
			foreach (var element in this.sequence)
			{
				answer+= element+ " ";
			}
			return answer;
		}
		
		public bool isGraphicSequence(List<int> sequence, int lengthSequence)
		{
			int w = lengthSequence + 1, b = 0, s = 0, c = 0;
			for (var i = 0; i <= lengthSequence; i++)
			{
				b += sequence[i];
				c = c + w - 1;
				while (w -1 > i && sequence[w-1] <= i)
				{
					s += sequence[w-1];
					c -= i + 1;
					w--;
				}
				if (b > c + s)
				{
					return false;
				} 
				if (w-1 == i)
				{
					return true;
				}
			}
			return false;
			
		}
		
		public int[,] getAdjacencyMatrix(List<int> sequence, int lengthSequence)
		{
			var newSequence = (List<int>)Clone(sequence);
			var adjacencyMatrix = new int [lengthSequence,lengthSequence];
			for(var i = 0; i < lengthSequence; i++)
			{
				var degree = newSequence[i];
				var index = 0;
				while (degree > 0 && index <lengthSequence)
				{	
					var sum = 0;
					if ( index!= i)
					{
						for(var j = 0; j < lengthSequence; j++)
						{
							sum+=adjacencyMatrix[index, j];
						}
						if(sum < sequence[index])
						{
								
							adjacencyMatrix[i, index] = 1;
							adjacencyMatrix[index, i] = 1;
							newSequence[index]-= 1;
							degree--;
						}
						
					}
					index++;
				}
	
			}
			return adjacencyMatrix;
		}
		public Queue<NodeSequence> getQueue(int indexReduction, int indexIncrease, NodeSequence node, Queue<NodeSequence> queue)
		{
			List<int> sequence = node.sequence;
			var lengthSequence = sequence.FindLastIndex(x=>x!=0);
			if (indexIncrease != indexReduction && indexReduction > 0 && indexReduction < lengthSequence
			    && sequence[indexReduction] - 1 >= sequence[indexReduction+1]
			    || indexReduction == lengthSequence 
			        && sequence[indexIncrease] - sequence[indexReduction] == 0 
			    || indexReduction == lengthSequence  && indexReduction - indexIncrease == 1)
			{
				if(indexIncrease > 0 && sequence[indexIncrease-1] >= sequence[indexIncrease] + 1 || indexIncrease == 0 )
				{
					List<int> newSequence = Clone(sequence);
					newSequence[indexReduction]-= 1;
					newSequence[indexIncrease]+= 1;
					if (isGraphicSequence(newSequence, lengthSequence))
					{
						var l = newSequence.FindLastIndex(x=>x!=0);
						int[,] matrix = getAdjacencyMatrix(newSequence, l +1);
						var nodeSequence = new NodeSequence(newSequence);;
						node.children.Add(nodeSequence);
						nodeSequence.parents.Add(new Tuple<int, int>(indexIncrease, indexReduction), node);
						if(!used.Contains(nodeSequence.name))
						{
							used.Add(nodeSequence.name);
							nodeSequence.matrix = matrix;
							queue.Enqueue(nodeSequence);

						} 
					}
				}
			}
			
			return queue;
		}
	
		public void bfs (NodeSequence nodeSequence)
		{ 
			Queue<NodeSequence> queue = new Queue<NodeSequence>();
			queue.Enqueue(nodeSequence);
			while (queue.Count != 0)
			{
				NodeSequence currentSequence = queue.Dequeue();
				var lengthSequence = currentSequence.sequence.FindLastIndex(x=>x!=0);
				for (var i = 0; i < lengthSequence ; i++)
				{
	
						var index = currentSequence.sequence.FindIndex(x=>x<currentSequence.sequence[i]);
						queue = getQueue(index, i, currentSequence, queue);
					 	index = currentSequence.sequence.FindLastIndex(x=>x==currentSequence.sequence[i]);
						queue = getQueue(index, i, currentSequence, queue);
					 	index = currentSequence.sequence.FindLastIndex(x=>x!=currentSequence.sequence[i]);
					 	queue = getQueue(index, i, currentSequence, queue);
					 	if(i == lengthSequence - 1 && currentSequence.children.Count == 0)
					 	{					 	
					 		nodeSequence.maximumGraphicsSequence.Add(currentSequence);
					 	} 
				}
			}
		}
		
		public Queue<NodeSequence> generatingThresholdGraphs()
        {
			var queue =  new Queue<NodeSequence>();
            foreach (var maxSequence in maximumGraphicsSequence)
            {
                var graph = new AdjacencyGraph<int, Edge<int>>();
                var matrix = Clone(maxSequence.matrix);
                var length = matrix.GetLength(0);
                for(var i = 1; i <= length; i++)
                {
                    graph.AddVertex(i);
                    for(var j = 0; j < length; j++)
                    {
                        if (matrix[i-1, j] == 1)
                        {
                            matrix[j, i-1] = 0;
                            var edge = new Edge<int>(i, j+1);
                            graph.AddEdge(edge);
                        }
                    }
                }
                maxSequence.graphs.Add(graph);
                queue.Enqueue(maxSequence);
           
            }
            return queue;
        }
		public void drawingSequence(Queue<NodeSequence> sequences)
        {
            
            while(sequences.Count != 0)
            {
            	NodeSequence sequence = sequences.Dequeue();
            	foreach (var graph in sequence.graphs)
            	{
	                var graphViz = new GraphvizAlgorithm<int, Edge<int>>(graph, @".\", GraphvizImageType.Png);
	                graphViz.FormatVertex += FormatVertex;
	                graphViz.FormatEdge += FormatEdge;
	                graphViz.Generate(new FileDotEngine(), sequence.name);
                   	foreach (var parent in sequence.parents)
                    {
	                    var copyGraph = graph.Clone();
	                    var soure = parent.Key.Item1;
	                    var target = parent.Key.Item2;
	                   	if (copyGraph.VertexCount < soure) copyGraph.AddVertex(soure);
	                    for (var i = 0; i < sequence.matrix.GetLength(0); i++)
	                    {
	                        if (sequence.matrix[soure, i] == 1 && i != target)
	                        {
	                            copyGraph.AddEdge(new Edge<int>(target + 1, i + 1));
	                            foreach (var edge in copyGraph.Edges)
	                            {
	                            	if (edge.Source == soure + 1 && edge.Target == i + 1 || edge.Source == i + 1 && edge.Target == soure + 1)
	                              	{
	                                	copyGraph.RemoveEdge(edge);
	                                	break;
	                               	}
	                            }
	                            
	                            break; 
	                          }
	                    }
	                    parent.Value.graphs.Add(copyGraph);
	                    sequences.Enqueue(parent.Value);
                    } 
                   	
                }
            	
            }
         }  
        


		
//		public void DrawingSequences(Queue<NodeSequence> queue)
//		{
//			while(queue.Count != 0)
//			{
//				var sequence  = queue.Dequeue();
//                var g = new AdjacencyGraph<int, Edge<int>>();
//                var matrix = Clone(sequence.matrix);
//                for(var i = 1; i <= matrix.GetLength(0); i++)
//                {
//                 	g.AddVertex(i);
//                 	
//                    for(var j = 0; j < matrix.GetLength(0); j++)
//                    {
//                    	if (matrix[i-1, j] == 1)
//                    	{
//                        	matrix[j, i-1] = 0;
//                           	var edge = new Edge<int>(i, j+1);
//                            g.AddEdge(edge);
//                    	}
//                    }
// 
//                }
//               
//                
//				var graphViz = new GraphvizAlgorithm<int, Edge<int>>(g, @".\", GraphvizImageType.Png);
//                graphViz.FormatVertex += FormatVertex;
//                graphViz.FormatEdge += FormatEdge;
//                graphViz.Generate(new FileDotEngine(), sequence.name);
//                
//			}
//		}
		        
        private static void FormatVertex(object sender, FormatVertexEventArgs<int> e)
        {
            
            e.VertexFormatter.Shape = GraphvizVertexShape.Point;
        }
        private static void FormatEdge(object sender, FormatEdgeEventArgs<int, Edge<int>> e)
        {

            e.EdgeFormatter.Dir = GraphvizEdgeDirection.None;

        }
	}
}
