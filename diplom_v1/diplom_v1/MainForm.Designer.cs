/*
 * Created by SharpDevelop.
 * User: Татьяна
 * Date: 04.04.2017
 * Time: 19:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace diplom_v1
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Label labelGraphWeight;
		private System.Windows.Forms.TextBox graphWeight;
		private System.Windows.Forms.Button generateGraphs;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelGraphWeight = new System.Windows.Forms.Label();
			this.graphWeight = new System.Windows.Forms.TextBox();
			this.generateGraphs = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// labelGraphWeight
			// 
			this.labelGraphWeight.Location = new System.Drawing.Point(13, 41);
			this.labelGraphWeight.Name = "labelGraphWeight";
			this.labelGraphWeight.Size = new System.Drawing.Size(95, 37);
			this.labelGraphWeight.TabIndex = 0;
			this.labelGraphWeight.Text = "Введите вес графа";
			this.labelGraphWeight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// graphWeight
			// 
			this.graphWeight.Location = new System.Drawing.Point(114, 50);
			this.graphWeight.Name = "graphWeight";
			this.graphWeight.Size = new System.Drawing.Size(100, 20);
			this.graphWeight.TabIndex = 1;
			this.graphWeight.TextChanged += new System.EventHandler(this.GraphWeightTextChanged);
			// 
			// generateGraphs
			// 
			this.generateGraphs.Location = new System.Drawing.Point(85, 124);
			this.generateGraphs.Name = "generateGraphs";
			this.generateGraphs.Size = new System.Drawing.Size(100, 23);
			this.generateGraphs.TabIndex = 2;
			this.generateGraphs.Text = "Сгенерировать";
			this.generateGraphs.UseVisualStyleBackColor = true;
			this.generateGraphs.Click += new System.EventHandler(this.GenerateGraphsClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(254, 226);
			this.Controls.Add(this.generateGraphs);
			this.Controls.Add(this.graphWeight);
			this.Controls.Add(this.labelGraphWeight);
			this.Name = "MainForm";
			this.Text = "diplom_v1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}
}
