using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace DesignPatternsV2._0
{
    public sealed partial class Form1 : Form
    {
        private enum Mode
        {
            Ellipse,
            Rectangle,
            Controller
        }

        //Variables
        private Mode _activemode = Mode.Controller;
        private Point _startLocation;
        private MoveInfo _moving;
        private ResizeInfo _resizing;
        private Draw _selected;

        private readonly Stack<ICommand> _commandStack = new Stack<ICommand>();
        private readonly Stack<ICommand> _undoneCommands = new Stack<ICommand>();

        private Point _mouseDownLocation;

        private readonly List<Draw> _shapeList;
        private readonly List<Draw> _compositeList = new List<Draw>(); 
        private readonly List<Draw> _undoneList;

        public Form1()
        {
            DoubleBuffered = true;

            Paint += Form1_Paint;
            MouseMove += Form1_MouseMove;
            MouseDown += Form1_MouseDown;
            MouseUp += Form1_MouseUp;

            _shapeList = new List<Draw>();

            KeyPreview = true;
            KeyDown += Form1_KeyDown;

            _undoneList = new List<Draw>();

            InitializeComponent();
            UpdateControls();
        }



        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_moving != null)
                {
                    MoveCommand moveCommand = new MoveCommand(_shapeList, _moving.Shape, _moving.StartMoveMousePoint,
                        e.Location);
                    //moveCommand.Do();
                    _commandStack.Push(moveCommand);
                    _undoneCommands.Clear();
                    UpdateControls();
                    Invalidate();
                    Capture = false;
                    _moving = null;
                }
                if (_resizing != null)
                {
                    ResizeCommand resizeCommand = new ResizeCommand(_resizing.Shape, _mouseDownLocation, e.Location);
                    //moveCommand.Do();                    
                    _commandStack.Push(resizeCommand);
                    _undoneCommands.Clear();
                    UpdateControls();
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
                            DrawCommand rectangleCommand = new DrawCommand(_shapeList,
                                new DrawRectangle(_startLocation.X, _startLocation.Y, e.X, e.Y));
                            rectangleCommand.Do();
                            _commandStack.Push(rectangleCommand);
                            _undoneCommands.Clear();
                        }
                        
                        UpdateControls();
                        Invalidate();
                        break;
                    case Mode.Ellipse:
                        if (_startLocation.X != e.X && _startLocation.Y != e.Y)
                        {
                            DrawCommand ellipseCommand = new DrawCommand(_shapeList,
                                new DrawEllipse(_startLocation.X, _startLocation.Y, e.X, e.Y));
                            ellipseCommand.Do();
                            _commandStack.Push(ellipseCommand);
                            _undoneCommands.Clear();
                        }
                        
                        UpdateControls();
                        Invalidate();
                        break;
                } //end mode switch
            } //end leftclick
            else if (e.Button == MouseButtons.Right)
            {
                if (_activemode == Mode.Controller)
                {
                    SetupRightClickmenu();
                }
            }

        }

        private void SetupRightClickmenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem toFront = new MenuItem("Move item to top");
            MenuItem toBack = new MenuItem("Move item to bottom");
            MenuItem oneUp = new MenuItem("Move item one layer up");
            MenuItem oneDown = new MenuItem("Move item one layer down");

            cm.MenuItems.Add(toFront);
            cm.MenuItems.Add(toBack);

            toFront.Click += toFront_click;
            toBack.Click += toBack_click;
            oneUp.Click += oneUp_click;
            oneDown.Click += oneDown_click;

            cm.MenuItems.Add(toFront);
            cm.MenuItems.Add(toBack);
            cm.Show(this,_mouseDownLocation);
        }

        void toFront_click(object sender, EventArgs e)
        {
            //MessageBox.Show(@"move object to front");
            if (_selected != null)
            {
                //int index = _shapeList.IndexOf(_selected);
                //Draw tmp = _shapeList[index];//get selected shape
                //_shapeList.Insert(0, tmp);
                //_shapeList.RemoveAt(index);
                
            }
        }

        void toBack_click(object sender, EventArgs e)
        {
            MessageBox.Show(@"move object to back");
        }

        void oneUp_click(object sender, EventArgs e)
        {
            MessageBox.Show(@"move object to back");
        }

        void oneDown_click(object sender, EventArgs e)
        {
            MessageBox.Show(@"move object to back");
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (_activemode == Mode.Controller)
            {
                _mouseDownLocation = e.Location;
                if (_selected != null && _moving == null)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        if (GetHandleRectangle(i, _selected).Contains(e.Location))
                        {
                            _resizing = new ResizeInfo
                            {
                                Shape = _selected,
                                StartResizePoint = _selected.StartPoint,
                                HandleNumber = i
                            };
                            break;
                        }
                        Capture = true;
                        
                        _moving = new MoveInfo
                        {
                            Shape = _selected,
                            StartShapePoint = _selected.StartPoint,
                            EndShapePoint = _selected.EndPoint,
                            StartMoveMousePoint = e.Location
                        };
                    }
                    //Capture = true;
                    //_moving = new MoveInfo
                    //{
                    //    Shape = _selected,
                    //    StartShapePoint = _selected.StartPoint,
                    //    EndShapePoint = _selected.EndPoint,
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
            foreach (var shape in _shapeList)
            {
                Color color;
                if (shape == _selected)
                {
                    color = Color.Red;
                    drawResizeHandles(_selected, e.Graphics);
                }
                else
                {
                    color = Color.Black;
                }
                var pen = new Pen(color, 1);
                if (shape is DrawRectangle)
                {
                    e.Graphics.DrawRectangle(pen, GetNormalizedRectangle(shape.StartPoint, shape.EndPoint));
                }
                else if (shape is DrawEllipse)
                {
                    e.Graphics.DrawEllipse(pen, GetNormalizedRectangle(shape.StartPoint, shape.EndPoint));
                }
                else if (shape is Composite)
                {
                    //recursive for children
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
                //RefreshShapeSelection(e.Location);
                SetCursor(e.Location);
                Invalidate();
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
            var selectedShape = FindShapeByPoint(_shapeList, point);
            if (selectedShape != _selected)
            {
                _selected = selectedShape;
                Invalidate();
            }
            if (_moving != null)
                Invalidate();
            
        }

        private void SetCursor(Point point)
        {
            if (_selected != null)
            {
                if (_selected.GetNormalizedRectangle().Contains(point))
                {
                    Cursor = Cursors.SizeAll;
                }
                else
                {
                    Cursor = Cursors.Default;
                }
            }
            
        }

        public Rectangle GetHandleRectangle(int handleNumber, Draw shape)
        {
            Point point = GetHandle(handleNumber, shape);

            return new Rectangle(point.X - 3, point.Y - 3, 7, 7);
        }

        public Point GetHandle(int handleNumber, Draw shape)
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

        private void drawResizeHandles(Draw shape, Graphics e)
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

        private static Draw FindShapeByPoint(List<Draw> shapes, Point p)
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

        private static Rectangle GetNormalizedRectangle(Point p1, Point p2)
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

        private void UpdateControls()
        {
            button2.Enabled = _undoneCommands.Count >= 1;

            button1.Enabled = _commandStack.Count >= 1;
        }

        private void ClearAll()
        {
            _compositeList.Clear();
            _commandStack.Clear();
            _undoneCommands.Clear();
            _shapeList.Clear();
            _undoneList.Clear();
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
            ICommand undoneCommand = _commandStack.Pop();
            undoneCommand.Undo();
            _undoneCommands.Push(undoneCommand);

            _selected = null;
            //Draw undoneShape = ShapeList[ShapeList.Count - 1];
            //UndoneList.Add(undoneShape);
            //ShapeList.RemoveAt(ShapeList.Count() - 1);
            Invalidate();
            UpdateControls();
        }
        //redo button clicked
        private void button2_Click(object sender, EventArgs e)
        {

            ICommand redoCommand = _undoneCommands.Pop();
            redoCommand.Do();
            _commandStack.Push(redoCommand);

            //ShapeList.Add(UndoneList[UndoneList.Count() - 1]);
            //UndoneList.RemoveAt(UndoneList.Count() - 1);

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
            foreach (Draw s in _shapeList)
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
                            //try
                            //{
                                string[] data = line.Split(' ');
                                if (((data[0])[0]).ToString() == "E")
                                {
                                    Draw shape = new DrawEllipse((Int16.Parse(data[1])), (Int16.Parse(data[2])), (Int16.Parse(data[3])), (Int16.Parse(data[4])));
                                    _compositeList.Add(shape);
                                    
                                    //shapeList.Add(composite);
                                }
                                else if (((data[0])[0]).ToString() == "R")
                                {
                                    Draw shape = new DrawRectangle((Int16.Parse(data[1])), (Int16.Parse(data[2])), (Int16.Parse(data[3])), (Int16.Parse(data[4])));
                                    _shapeList.Add(shape);
                                }
                                UpdateControls();
                                _selected = null;
                            //}
                            //catch
                            //{
                            //    MessageBox.Show(@"shit went terribly wrong");
                            //    ClearAll();
                            //    UpdateControls();
                            //    Invalidate();
                            //    break;
                            //}
                        }
                        Composite composite = new Composite(_compositeList, _shapeList);
                        composite.AddToList();
                    }
                    break;
            }
        }

        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                RemoveCommand removeCommand = new RemoveCommand(_shapeList,_selected);
                removeCommand.Do();
                _commandStack.Push(removeCommand);
                UpdateControls();
                Invalidate();
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            int counter = 0;
            foreach (ICommand c in _undoneCommands)
            {
                Console.WriteLine(counter+c.GetType().ToString());
                counter++;
            }
            
        }
    }
}
