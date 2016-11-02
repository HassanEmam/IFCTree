using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xbim.Ifc;
using Xbim.Ifc4.Interfaces; //IFC4 interfaces are also implemented in our IFC2x3 schema implementation


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
            var editor = new XbimEditorCredentials
            {
                ApplicationDevelopersName = "You",
                ApplicationFullName = "Your app",
                ApplicationIdentifier = "Your app ID",
                ApplicationVersion = "4.0",
                //your user
                EditorsFamilyName = "Emam",
                EditorsGivenName = "Hassan",
                EditorsOrganisationName = "Logikal"
            };
            using (var model = IfcStore.Open(fileName, editor, true))
            {
                using (var txn = model.BeginTransaction("Quick start transaction"))
                {
                    //get all walls in the model
                    var bldgElements = model.Instances.OfType<IIfcBuildingElement>();
                    var stories = model.Instances.OfType<IIfcBuildingStorey>();
                    //iterate over all the walls and change them
                    foreach (var story in stories)
                    {
                        Console.WriteLine(story.Name);
                        TreeNode level = new TreeNode();
                        level.Text= story.Name;
                        treeView1.Nodes.Add(level);
                        foreach (var element in bldgElements)
                        {
                            //story.Name = story.Name;
                            //IIfcPhysicalSimpleQuantity area = story.
                            //Console.WriteLine(story.IsDefinedBy.ToString());
                            if (element.IsContainedIn == story)
                            {
                                TreeNode child = new TreeNode();
                                child.Text = element.Name;
                                level.Nodes.Add(child);
                                Console.WriteLine(element.Name + " " + element.IsContainedIn.Name + " " + element.GetType().Name);
                            }

                            //Console.WriteLine("\t" + wall.ConnectedFrom.ToString() + story);
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
