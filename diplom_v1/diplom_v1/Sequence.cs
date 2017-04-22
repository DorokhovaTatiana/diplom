/*
 * Created by SharpDevelop.
 * User: Татьяна
 * Date: 15.04.2017
 * Time: 21:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using QuickGraph.Graphviz.Dot;
using QuickGraph;
using QuickGraph.Graphviz;
using System.Linq;
using System.IO;
using GraphX.PCL.Logic.Algorithms;

namespace diplom_v1
{
	/// <summary>
	/// Description of Sequence.
	/// </summary>
	/// 
	[Serializable]
	public class Sequence
	{
		public int length { get; set; }
		public string name { get; set; }
		public int[,] matrix { get; set; }
		public List<int> sequence { get; set; }
		public Dictionary<Tuple<int, int>, Sequence> parents { get; set; }
		public List<Sequence> graphs { get; set; }
		public bool isChildren { get; set; }
		public Dictionary<Tuple<int, int>, Sequence> indexes { get; set; }
		public List<string> drawing  { get; set; }
		
		public List<Sequence> maximumGraphicsSequence = new List<Sequence>();
		public List<Sequence> sequences = new List<Sequence>();
		public List<string> used = new List<string>();
		
		int index = 0;
		
		public Sequence(int weight)
		{
			this.matrix = null;
			this.length = weight;
			this.isChildren = false;
			this.sequence = generatingList(weight);
			this.name = String.Join(",", this.sequence);
			this.indexes = new Dictionary<Tuple<int, int>, Sequence>();
			this.graphs = new List<Sequence>();
			this.drawing = new List<string>();
			
		}
		public Sequence(List<int> sequence)
		{
			this.matrix = null;
//			this.per = null;
			this.isChildren = false;
			this.sequence = sequence;
			this.name = String.Join(",", sequence);
			this.length = sequence.FindIndex(x=>x==0);
			this.indexes = new Dictionary<Tuple<int, int>, Sequence>();
			this.graphs = new List<Sequence>();
			this.drawing = new List<string>();
		}
		public List<int> generatingList(int weight)
		{
			var list = new List<int>();
			for(var i = 0; i < weight; i++)
			{
				list.Add(1);
			}
			return list;
		}
		public T Clone<T>(T source)
	    {
//	        if (!typeof(T).IsSerializable)
//	        {
//	            throw new ArgumentException("The type must be serializable.", "source");
//	        }
	
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
			int w = lengthSequence, b = 0, s = 0, c = 0;
			for (var i = 0; i < lengthSequence; i++)
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
		public Queue<Sequence> getQueue(int indexReduction, int indexIncrease, Sequence node, Queue<Sequence> queue)
		{
			List<int> sequence = node.sequence;
			if (indexIncrease != indexReduction && indexReduction > 0 && indexReduction < node.length-1
			    && sequence[indexReduction] - 1 >= sequence[indexReduction+1]
			    || indexReduction == node.length - 1
			        && sequence[indexIncrease] - sequence[indexReduction] == 0 
			    || indexReduction == node.length - 1 && indexReduction - indexIncrease == 1)
			{
				if(indexIncrease > 0 && sequence[indexIncrease-1] >= sequence[indexIncrease] + 1 || indexIncrease == 0 )
				{
					List<int> copySequence = Clone(sequence);
					copySequence[indexReduction]-= 1;
					copySequence[indexIncrease]+= 1;
					if (isGraphicSequence(copySequence, node.length))
					{
						var newSequence = new Sequence(copySequence);
						node.isChildren = true;
						if(!used.Contains(newSequence.name))
						{
							used.Add(newSequence.name);
							sequences.Add(newSequence);
							queue.Enqueue(newSequence);
						}
						else
						{
							newSequence = sequences.Where(s=>s.name == newSequence.name).First();
						}
						newSequence.indexes.Add(new Tuple<int, int>(newSequence.sequence[indexIncrease], newSequence.sequence[indexReduction]),node);
//		
					}
				}
			}
			
			
			return queue;
		}
	
		public void bfs ()
		{ 
			Queue<Sequence> queue = new Queue<Sequence>();
			queue.Enqueue(this);
			sequences.Add(this);
			while (queue.Count != 0)
			{
				Sequence currentSequence = queue.Dequeue();
				for (var i = 0; i <  currentSequence.length - 1; i++)
				{
	
						var index = currentSequence.sequence.FindIndex(x=>x<currentSequence.sequence[i]);
						queue = getQueue(index, i, currentSequence, queue);
					 	index = currentSequence.sequence.FindLastIndex(x=>x==currentSequence.sequence[i]);
						queue = getQueue(index, i, currentSequence, queue);
					 	index = currentSequence.sequence.FindLastIndex(x=>x!=currentSequence.sequence[i]);
					 	queue = getQueue(index, i, currentSequence, queue);
					 	if(i == currentSequence.length - 2 && !currentSequence.isChildren)
					 	{					 	
					 		maximumGraphicsSequence.Add(currentSequence);
					 	} 
				}
			}
		}
		
		public Queue<Sequence> generatingMaximumGraphs()
		{
			var sequences = new Queue<Sequence>();
			foreach (var maxSequence in maximumGraphicsSequence)
            {
                sequences.Enqueue(maxSequence);
            }
			return sequences;
		}
		public void drawingSequence(Sequence sequence)
		{
			sequence.matrix = getAdjacencyMatrix(sequence.sequence, sequence.length);
			var matrix = sequence.matrix;
			var g = new AdjacencyGraph<int, Edge<int>>();
			var length = sequence.length;
            for(var i = 1; i <= length; i++)
            {
                g.AddVertex(i);
                for(var j = 0; j < length; j++)
                {
                    if (matrix[i-1, j] == 1)
                   	{
                        matrix[j, i-1] = 0;
                        var edge = new Edge<int>(i, j+1);
                       	g.AddEdge(edge);
                    }
                 }
            }
			var graphViz = new GraphvizAlgorithm<int, Edge<int>>(g, @".\", GraphvizImageType.Png);
	        graphViz.FormatVertex += FormatVertex;
	        graphViz.FormatEdge += FormatEdge;
	        graphViz.Generate(new FileDotEngine(), sequence.name);
	        
//	        return g;
		}
		public void drawingAllSequence(Queue<Sequence> sequences)
        {
			while(sequences.Count > 0)
			{
				var sequence = sequences.Dequeue();
				drawingSequence(sequence);
				foreach(var e in sequence.indexes.Keys)
				{
					int increase = e.Item1, reduction = e.Item2;
					
					var listSequence = getNewSequence(sequence, increase, reduction, sequence.indexes[e]);
					listSequence.ForEach(s=> sequences.Enqueue(s));
				}
			}
            
			
		}
		public List<Sequence> getNewSequence(Sequence seq, int sourse, int target, Sequence parent)
		{
			var sequences = new List<Sequence>();
			var sequence = seq.sequence;
			var length = seq.length;
			length = target > length ? target: length;
			for (var i = 0; i <= length; i++)
		    { 
				for(var j = 0; j <= length; j++)
				{
					if (i != j && sourse == sequence[i] && sequence[j] == target)
					{
						var copy = Clone(sequence);
						copy[j] +=1;
						copy[i] -=1;
						var newSeq = new Sequence(copy);

						if(!seq.drawing.Contains(newSeq.name))
						{
							var copyIndexes = Clone(parent.indexes);
							newSeq.indexes = copyIndexes;
							sequences.Add(newSeq);
							seq.drawing.Add(newSeq.name);
						}
						break;	
					}
				}
		        
	         }
			return sequences;
		}
//		public List<Sequence> getPermutations(Sequence sequence)
//		{
//			var name = sequence.name;
//			var length = sequence.length;
//			List<Sequence> shuffled = new List<Sequence>();
//			char[] wordArray = name.ToCharArray();
//            char symbol;
//            for (int i = 0; i < length*2 - 1; i+=2)
//            {
//                for (int j = 0; j < length*2 - 1; j+=2)
//                {
//                    symbol = wordArray[j];                    
//                    wordArray[j] = wordArray[j + 2];
//                    wordArray[j + 2] = symbol;
//                    var version = String.Join("", wordArray);
//                    if (!shuffled.Contains(version)) shuffled.Add(version);
//                }
//            }
//            return shuffled;
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
