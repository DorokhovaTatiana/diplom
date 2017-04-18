/*
 * Created by SharpDevelop.
 * User: Татьяна
 * Date: 04.04.2017
 * Time: 19:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using QuickGraph.Algorithms;
using System.Windows.Forms;
using System.IO;



namespace diplom_v1
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class MainForm : Form
    {
        public MainForm()
        {

            InitializeComponent();
            
        }
        void GraphWeightTextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(graphWeight.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                graphWeight.Text = graphWeight.Text.Remove(graphWeight.Text.Length - 1);
                graphWeight.Focus();
                graphWeight.SelectionStart = graphWeight.Text.Length;
            }
        }
       
        void GenerateGraphsClick(object sender, EventArgs e)
        {
            if(graphWeight.Text != "")
            {
                var weight = int.Parse(graphWeight.Text);
                var sequence = new Sequence(weight);
                sequence.bfs();
               
            }
        }
    }
}
