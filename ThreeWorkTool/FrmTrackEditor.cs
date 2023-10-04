using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreeWorkTool.Resources.Wrappers;
using ThreeWorkTool.Resources.Wrappers.AnimNodes;

namespace ThreeWorkTool
{
    public partial class FrmTrackEditor : Form
    {
        private static ThreeSourceTree treeview;
        private static TreeNode node_;
        public LMTM3AEntry M3a;
        public LMTTrackNode tracks;
        public bool EntryChanged = false;
        public FrmMainThree Mainfrm { get; set; }
        public bool isModified = false;
        public BindingList<LMTTrackNode> TracksList;

        public FrmTrackEditor()
        {
            InitializeComponent();
        }

        public void FillTheListGrid()
        {
            for (int i = 0; i < M3a.TrackCount; i++)
            {
                TrackGridView.Rows.Add(M3a.Tracks[i].TrackNumber, M3a.Tracks[i].BufferType, M3a.Tracks[i].BoneID, M3a.Tracks[i].TrackType, M3a.Tracks[i].ReferenceData.X,
                    M3a.Tracks[i].ReferenceData.Y, M3a.Tracks[i].ReferenceData.Z, M3a.Tracks[i].ReferenceData.W, M3a.Tracks[i].RefDataPointer);
            }
        }

        public DataGridViewRow DGViewTemplateMaker(DataGridViewRow DGVR)
        {





            return DGVR;

        }

        public void ShowTrackEditor(TreeNode tn)
        {
            treeview = Mainfrm.TreeSource;
            node_ = treeview.SelectedNode;
            if (tn.Tag as LMTM3AEntry != null)
            {
                M3a = tn.Tag as LMTM3AEntry;
                if (M3a.IsBlank == false)
                {

                    this.Text = "Track Editor - " + tn.Text;
                    FillTheListGrid();
                    this.ShowDialog();
                }
                else
                {
                    MessageBox.Show("This animation is blank so there is no track data to show.", "Empty Animation is Empty");
                }
            }
        }

        private void TrackGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var row = TrackGridView.Rows[e.RowIndex];
            if (null != row)
            {
                var cell = row.Cells[e.ColumnIndex];
                if (null != cell)
                {
                    object value = cell.Value;
                    if (null != value)
                    {
                        if (cell.ColumnIndex == 4 || cell.ColumnIndex == 5 || cell.ColumnIndex == 6 || cell.ColumnIndex == 7)
                        {
                            string TestStr = value.ToString();
                            bool IsFloat = Regex.IsMatch(TestStr, @"^[a-zA-Z]+$");
                            if (IsFloat == true)
                            {
                                MessageBox.Show("The entered values must be in floating point.\nThe highlighted value will not save properly like this.");
                            }
                        }

                        if (cell.ColumnIndex == 1 || cell.ColumnIndex == 2 || cell.ColumnIndex == 3)
                        {
                            string TestStr = value.ToString();
                            bool IsFloat = Regex.IsMatch(TestStr, @"^[a-zA-Z]+$");
                            if (IsFloat == true)
                            {
                                MessageBox.Show("The entered values must be a valid integer.\nThe highlighted value will not save properly like this.");
                            }

                            int TestValue = Convert.ToInt32(value);
                            if(TestValue > 255)
                            {
                                MessageBox.Show("The entered value cannot be more 255 because this is stored as a byte.");
                            }


                        }


                    }
                }
            }
        }

        private void FrmTrackEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            int BufferType = 0;
            int BoneID = 0;
            int TrackType = 0;
            float W = 0;
            float X = 0;
            float Y = 0;
            float Z = 0;
            int PointerToUse = 0;
            byte BufferTypeByte;
            byte BoneIDByte;
            byte TrackIDByte;
            int Counter = 0;
            byte[] RawDataBackup = new byte[M3a.RawData.Length];
            RawDataBackup = M3a.RawData;
            //Update values in M3a here.
            //foreach (DataGridViewRow row in TrackGridView.Rows)
            for (int v = 0; v < (TrackGridView.Rows.Count -1); v++)
            {
                //Gets the values.
                BufferType = Convert.ToInt32(TrackGridView.Rows[v].Cells[1].Value);
                BoneID = Convert.ToInt32(TrackGridView.Rows[v].Cells[2].Value);
                TrackType = Convert.ToInt32(TrackGridView.Rows[v].Cells[3].Value);
                X = Convert.ToSingle(TrackGridView.Rows[v].Cells[4].Value);
                Y = Convert.ToSingle(TrackGridView.Rows[v].Cells[5].Value);
                Z = Convert.ToSingle(TrackGridView.Rows[v].Cells[6].Value);
                W = Convert.ToSingle(TrackGridView.Rows[v].Cells[7].Value);
                PointerToUse = Convert.ToInt32(TrackGridView.Rows[v].Cells[8].Value) - Convert.ToInt32(M3a.TrackPointer);

                if(BoneID > 255 || TrackType > 255 || BufferType > 255)
                {
                    MessageBox.Show("There's an invalid value for either the BufferType, BoneID, or the TrackID so this will NOT be saved.");
                    M3a.RawData = RawDataBackup;
                    TrackGridView.Rows.Clear();
                    return;

                }

                if (PointerToUse < 0)
                {
                    MessageBox.Show("The track pointers are bogus so changes won't be saved.");
                    return;
                }
                else
                {
                    BufferTypeByte = Convert.ToByte(BufferType);
                    BoneIDByte = Convert.ToByte(BoneID);
                    TrackIDByte = Convert.ToByte(TrackType);
                    //Writes them to the M3a.
                    using (MemoryStream ms3 = new MemoryStream(M3a.RawData))
                    {
                        using (BinaryReader br3 = new BinaryReader(ms3))
                        {
                            using (BinaryWriter bw3 = new BinaryWriter(ms3))
                            {
                                bw3.BaseStream.Position = PointerToUse;
                                bw3.Write(W);
                                bw3.Write(X);
                                bw3.Write(Y);
                                bw3.Write(Z);
                                bw3.BaseStream.Position = (PointerToUse - 24);
                                bw3.Write(BufferTypeByte);
                                bw3.Write(TrackIDByte);
                                bw3.BaseStream.Position = (bw3.BaseStream.Position + 1);
                                bw3.Write(BoneIDByte);
                            }
                        }
                    }
                    //Writes the data to the variables.
                    M3a.Tracks[v].BufferType = BufferType;
                    M3a.Tracks[v].BoneID = BoneID;
                    M3a.Tracks[v].TrackType = TrackType;
                    M3a.Tracks[v].ReferenceData.W = W;
                    M3a.Tracks[v].ReferenceData.X = X;
                    M3a.Tracks[v].ReferenceData.Y = Y;
                    M3a.Tracks[v].ReferenceData.Z = Z;

                    Counter++;
                }
            }

            //Updates the Full M3a Data.
            Array.Copy(M3a.RawData, 0, M3a.FullData, 0, M3a.RawData.Length);

            //Empties the TrackGridView.
            TrackGridView.Rows.Clear();

            //Lastly, now to copy over the M3a to the one in the actually selected node.
            treeview.SelectedNode.Tag = M3a;
            EntryChanged = true;
        }
    }
}
