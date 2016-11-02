﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces; 


namespace IFCTree
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            const string fileName = "office.ifc"; //this can be either IFC2x3 or IFC4
            // editor credentials are required for IFCStore object
            var editor = new XbimEditorCredentials
            {
                ApplicationDevelopersName = "Hassan Emam",
                ApplicationFullName = "IFCTreeView",
                ApplicationIdentifier = "Version 01",
                ApplicationVersion = "4.0",
                //your user
                EditorsFamilyName = "Emam",
                EditorsGivenName = "Hassan",
                EditorsOrganisationName = "Emam Tech"
            };
            // Open the IFC file for reading the required data. Operations have to be performed within the using scope
            using (var model = IfcStore.Open(fileName, editor, true))
            {
                using (var txn = model.BeginTransaction("Quick start transaction"))
                {
                    //get all BuildingElements in the model
                    var bldgElements = model.Instances.OfType<IIfcBuildingElement>();
                    //get all Building Stories in the model
                    var stories = model.Instances.OfType<IIfcBuildingStorey>();
                    var project = model.Instances.FirstOrDefault<IIfcProject>();
                    TreeNode projectNode = new TreeNode();
                    projectNode.Text = project.Name.Value;
                    treeView1.Nodes.Add(projectNode);
                    //iterate over all the stories and add them to the treeView
                    foreach (var story in stories)
                    {
                        Console.WriteLine(story.Name);
                        TreeNode level = new TreeNode();
                        projectNode.Nodes.Add(level);
                        level.Text= story.Name;
                        //treeView1.Nodes.Add(level);
                        //iterate over all building elements and add to the level
                        foreach (var element in bldgElements)
                        {

                            if (element.IsContainedIn == story)
                            {
                                TreeNode child = new TreeNode();
                                child.Text = element.Name;
                                level.Nodes.Add(child);
                                Console.WriteLine(element.Name + " " + element.IsContainedIn.Name + " " + element.GetType().Name);
                            }
                        }
                    }

                    Console.Read();

                    //commit your changes
                    txn.Commit();
                }

                //save your changed model. IfcStore will use the extension to save it as *.ifc, *.ifczip or *.ifcxml.
                model.SaveAs("WallOnly-Modified.ifc");
            }
        }
    }
}
