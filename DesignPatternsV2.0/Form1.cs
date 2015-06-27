using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DesignPatternsV2._0
{
    public sealed partial class Form1 : Form
    {
        public enum Mode
        {
            None,
            Ellipse,
            Rectangle,
            Controller
        }

        //Variables
        private Mode _activemode = Mode.Controller;
        private Point _startLocation;
        private MoveInfo _moving;
        private ResizeInfo _resizing;
        private GraphShape _selectedShape;
        public List<GraphShape> ShapeList;
        public List<GraphShape> UndoneList;

        public Form1()
        {
            DoubleBuffered = true;

            Paint += Form1_Paint;
            MouseMove += Form1_MouseMove;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;

            ShapeList = new List<GraphShape>
            {
                new GraphShape(GraphShape.Shape.Rectangle, 200, 120, 300, 300),
                new GraphShape(GraphShape.Shape.Ellipse, 350, 250, 420, 140)
            };

            KeyPreview = true;
            KeyDown += Form1_KeyDown;

            UndoneList = new List<GraphShape>();

            InitializeComponent();
            UpdateControls();
        }



        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_moving != null)
            {
                Capture = false;
                _moving = null;
            }
            if (_resizing != null)
            {
                Capture = false;
                _resizing = null;
            }

            switch (_activemode)
            {
                case Mode.Controller:
                    RefreshShapeSelection(e.Location);
                    break;
                case Mode.Rectangle:
                    if (_startLocation.X != e.X && _startLocation.Y != e.Y)
                    {  
                        ShapeList.Add(new GraphShape(GraphShape.Shape.Rectangle, _startLocation.X, _startLocation.Y, e.X, e.Y));
                        UndoneList.Clear();
                    }
                    UpdateControls();
                    Invalidate();
                    break;
                case Mode.Ellipse:
                    if (_startLocation.X != e.X && _startLocation.Y != e.Y)
                    {
                        ShapeList.Add(new GraphShape(GraphShape.Shape.Ellipse, _startLocation.X, _startLocation.Y, e.X, e.Y));
                        UndoneList.Clear();
                    }
                    UpdateControls();
                    Invalidate();
                    break;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (_activemode == Mode.Controller)
            {

                if (_selectedShape != null && _moving == null)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        if (GetHandleRectangle(i, _selectedShape).Contains(e.Location))
                        {
                            _resizing = new ResizeInfo
                            {
                                Shape = _selectedShape,
                                StartResizePoint = _selectedShape.StartPoint,
                                HandleNumber = i
                            };
                            break;
                        }
                        Capture = true;
                        _moving = new MoveInfo
                        {
                            Shape = _selectedShape,
                            StartShapePoint = _selectedShape.StartPoint,
                            EndShapePoint = _selectedShape.EndPoint,
                            StartMoveMousePoint = e.Location
                        };
                    }
                    //Capture = true;
                    //_moving = new MoveInfo
                    //{
                    //    Shape = _selectedShape,
                    //    StartShapePoint = _selectedShape.StartPoint,
                    //    EndShapePoint = _selectedShape.EndPoint,
                    //    StartMoveMousePoint = e.Location
                    //};
                }
                RefreshShapeSelection(e.Location);
            }
            else if (_activemode == Mode.Rectangle)
            {
                _startLocation = e.Location;
            }

            else if (_activemode == Mode.Ellipse)
            {
                _startLocation = e.Location;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.High;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            foreach (var shape in ShapeList)
            {
                Color color;
                if (shape == _selectedShape)
                {
                    color = Color.Red;
                    drawResizeHandles(_selectedShape, e.Graphics);
                }
                else
                {
                    color = Color.Black;
                }
                var pen = new Pen(color, 1);
                if (shape.shape == GraphShape.Shape.Rectangle)
                {
                    e.Graphics.DrawRectangle(pen, GetNormalizedRectangle(shape.StartPoint, shape.EndPoint));
                }
                else if (shape.shape == GraphShape.Shape.Ellipse)
                {
                    e.Graphics.DrawEllipse(pen, GetNormalizedRectangle(shape.StartPoint, shape.EndPoint));
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_activemode == Mode.Controller)
            {

                if (_resizing != null)
                {
                    _resizing.Shape.StartPoint = new Point(e.X, e.Y);
                    Invalidate();
                }

                else if (_moving != null)
                {
                    _moving.Shape.StartPoint = new Point(_moving.StartShapePoint.X + e.X - _moving.StartMoveMousePoint.X,
                        _moving.StartShapePoint.Y + e.Y - _moving.StartMoveMousePoint.Y);
                    _moving.Shape.EndPoint = new Point(_moving.EndShapePoint.X + e.X - _moving.StartMoveMousePoint.X,
                        _moving.EndShapePoint.Y + e.Y - _moving.StartMoveMousePoint.Y);
                }
                RefreshShapeSelection(e.Location);
            }
            else if (_activemode == Mode.Rectangle)
            {

            }

            else if (_activemode == Mode.Ellipse)
            {

            }
        }

        private void RefreshShapeSelection(Point point)
        {
            var selectedShape = FindShapeByPoint(ShapeList, point);
            if (selectedShape != _selectedShape)
            {
                _selectedShape = selectedShape;
                Invalidate();
            }
            if (_moving != null)
                Invalidate();

            if (_moving != null)
            {
                Cursor = Cursors.Hand;
            }
            else if (_selectedShape != null)
            {
                Cursor = Cursors.SizeAll;
            }
            else
            {
                Cursor = Cursors.Default;
            }

        }

        public Rectangle GetHandleRectangle(int handleNumber, GraphShape shape)
        {
            Point point = GetHandle(handleNumber, shape);

            return new Rectangle(point.X - 3, point.Y - 3, 7, 7);
        }

        public Point GetHandle(int handleNumber, GraphShape shape)
        {
            Point point = new Point();

            switch (handleNumber)
            {
                case 1:
                    point = new Point(shape.GetNormalizedRectangle().Left, shape.GetNormalizedRectangle().Top);
                    break;
                case 2:
                    point = new Point(shape.GetNormalizedRectangle().Right, shape.GetNormalizedRectangle().Top);
                    break;
                case 3:
                    point = new Point(shape.GetNormalizedRectangle().Left, shape.GetNormalizedRectangle().Bottom);
                    break;
                case 4:
                    point = new Point(shape.GetNormalizedRectangle().Right, shape.GetNormalizedRectangle().Bottom);
                    break;

            }

            return point;
        }

        private void drawResizeHandles(GraphShape shape, Graphics e)
        {
            Point topLeft = new Point(shape.GetNormalizedRectangle().Left, shape.GetNormalizedRectangle().Top);
            Point topRight = new Point(shape.GetNormalizedRectangle().Right, shape.GetNormalizedRectangle().Top);
            Point bottomLeft = new Point(shape.GetNormalizedRectangle().Left, shape.GetNormalizedRectangle().Bottom);
            Point bottomRight = new Point(shape.GetNormalizedRectangle().Right, shape.GetNormalizedRectangle().Bottom);

            Rectangle topLeftRectangle = new Rectangle(topLeft.X - 4, topLeft.Y - 4, 8, 8);
            e.DrawRectangle(new Pen(Color.Red, 1), topLeftRectangle);

            Rectangle topRightRectangle = new Rectangle(topRight.X - 4, topLeft.Y - 4, 8, 8);
            e.DrawRectangle(new Pen(Color.Red, 1), topRightRectangle);

            Rectangle bottomLeftRectangle = new Rectangle(bottomLeft.X - 4, bottomLeft.Y - 4, 8, 8);
            e.DrawRectangle(new Pen(Color.Red, 1), bottomLeftRectangle);

            Rectangle bottomRightRectangle = new Rectangle(bottomRight.X - 4, bottomRight.Y - 4, 8, 8);
            e.DrawRectangle(new Pen(Color.Red, 1), bottomRightRectangle);
        }

        private static GraphShape FindShapeByPoint(List<GraphShape> shapes, Point p)
        {
            foreach (var shape in shapes)
            {
                Rectangle rect = shape.GetNormalizedRectangle();
                if (rect.Contains(p))
                {
                    return shape;
                }
            }
            return null;
        }

        public Rectangle GetNormalizedRectangle(Point p1, Point p2)
        {
            Rectangle normalizedRect = new Rectangle();
            if (p1.X < p2.X)
            {
                normalizedRect.X = p1.X;
                normalizedRect.Width = p2.X - p1.X;
            }
            else
            {
                normalizedRect.X = p2.X;
                normalizedRect.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                normalizedRect.Y = p1.Y;
                normalizedRect.Height = p2.Y - p1.Y;
            }
            else
            {
                normalizedRect.Y = p2.Y;
                normalizedRect.Height = p1.Y - p2.Y;
            }

            return normalizedRect;
        }

        public void UpdateControls()
        {
            button2.Enabled = UndoneList.Count >= 1;

            button1.Enabled = ShapeList.Count >= 1;
        }

        public void ClearAll()
        {
            ShapeList.Clear();
            UndoneList.Clear();
            UpdateControls();
            Invalidate();
        }


        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            _activemode = Mode.Ellipse;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            _activemode = Mode.Rectangle;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            _activemode = Mode.Controller;
        }

        //Undo button clicked
        private void button1_Click(object sender, EventArgs e)
        {
            GraphShape undoneShape = ShapeList[ShapeList.Count - 1];
            UndoneList.Add(undoneShape);
            ShapeList.RemoveAt(ShapeList.Count() - 1);
            Invalidate();
            UpdateControls();
        }
        //redo button clicked
        private void button2_Click(object sender, EventArgs e)
        {

            ShapeList.Add(UndoneList[UndoneList.Count() - 1]);
            UndoneList.RemoveAt(UndoneList.Count() - 1);

            Invalidate();
            UpdateControls();
        }

        //Save button clicked
        private void button3_Click(object sender, EventArgs e)
        {
            StreamWriter file = StreamWriter.Null;
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = @"txt files (*.txt)|*.txt|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true,
                DefaultExt = ".txt"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            try
            {
                file = new StreamWriter(sfd.FileName);
            }
            catch (Exception er)
            {
                Console.WriteLine(@"Error is : " + er);
            }
            //file = new System.IO.StreamWriter(sfd.FileName);
            foreach (GraphShape s in ShapeList)
            {
                string writetofile = s.shape + " " + s.StartPoint.X + " " + s.StartPoint.Y + " " + s.EndPoint.X + " " + s.EndPoint.Y;
                file.WriteLine(writetofile);
            }
            file.Close();
        }

        //Open button clicked
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result1 = MessageBox.Show(@"Are you sure you want to open a file? Your current document will be gone",
            @"Important Question",
            MessageBoxButtons.OKCancel);
            switch (result1)
            {
                case DialogResult.Cancel:

                    break;
                case DialogResult.OK:

                    ClearAll();

                    OpenFileDialog openFileDialog1 = new OpenFileDialog();
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        StreamReader file =
                            new StreamReader(openFileDialog1.FileName);
                        string line;
                        while ((line = file.ReadLine()) != null)
                        {
                            try
                            {
                                string[] data = line.Split(' ');
                                if (((data[0])[0]).ToString() == "E")
                                {
                                    GraphShape shape = new GraphShape(GraphShape.Shape.Ellipse, (Int16.Parse(data[1])), (Int16.Parse(data[2])), (Int16.Parse(data[3])), (Int16.Parse(data[4])));
                                    ShapeList.Add(shape);
                                }
                                else if (((data[0])[0]).ToString() == "R")
                                {
                                    GraphShape shape = new GraphShape(GraphShape.Shape.Rectangle, (Int16.Parse(data[1])), (Int16.Parse(data[2])), (Int16.Parse(data[3])), (Int16.Parse(data[4])));
                                    ShapeList.Add(shape);
                                }
                                UpdateControls();
                            }
                            catch
                            {
                                MessageBox.Show(@"shit went terribly wrong");
                                ShapeList.Clear();
                                UndoneList.Clear();
                                UpdateControls();
                                Invalidate();
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {

                ShapeList.Remove(_selectedShape);
                UpdateControls();
            }

        }
    }
}
