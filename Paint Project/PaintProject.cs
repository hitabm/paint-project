using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace Paint_Project
{
    public enum Choice { Rectangle, Ellipse, Line, Pencil, Eraser, Bucket, Select, Text }
    public partial class PaintProject : Form
    {
        Choice selectedAction = Choice.Pencil;
        int penWidth = 1, width, height;
        Point startPoint, sp;
        bool touched = false, opened = false, saved = false;
        Color selectedColor;
        Graphics graphic;
        Pen pen;
        Bitmap image, temp;
        static string opened_image_name;
        PictureBox pb;
        TextBox tb = new TextBox();

        public PaintProject()
        {
            InitializeComponent();
            NewPage();
        }

        #region Menu Bar
        private void ToolsButton_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            if (ToolBox.Visible == false)
            {
                ToolBox.Visible = true;
                DesignField.Location = new Point(12, 101);
                ToolsButton.Checked = true;
                ChangeSizeButton.Location = new Point(ChangeSizeButton.Location.X, ChangeSizeButton.Location.Y + 70);
                ChangeHeightButton.Location = new Point(ChangeHeightButton.Location.X, ChangeHeightButton.Location.Y + 70);
                ChangeWidthButton.Location = new Point(ChangeWidthButton.Location.X, ChangeWidthButton.Location.Y + 70);
            }
            else
            {
                ToolBox.Visible = false;
                DesignField.Location = new Point(12, 31);
                ToolsButton.Checked = false;
                ChangeSizeButton.Location = new Point(ChangeSizeButton.Location.X, ChangeSizeButton.Location.Y - 70);
                ChangeHeightButton.Location = new Point(ChangeHeightButton.Location.X, ChangeHeightButton.Location.Y - 70);
                ChangeWidthButton.Location = new Point(ChangeWidthButton.Location.X, ChangeWidthButton.Location.Y - 70);
            }
        }

        private void FileButton_Click(object sender, EventArgs e)
        {
            SizeMenu.Visible = false;
            if (FileMenu.Visible == true)
                FileMenu_Close(sender, e);
            else
            {
                FileMenu.Visible = true;
                FileButton.BackColor = Color.DarkBlue;
            }
        }

        private void InfoButton_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            MessageBox.Show("Paint v0.2\nBy Hita B. Mamagani\n2015-2023", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        #endregion

        #region File Menu
        void NewPage()
        {
            DesignField.Size = new Size(800, 450);
            image = new Bitmap(800, 450);
            graphic = Graphics.FromImage(image);
            graphic.Clear(Color.White);
            DesignField.Image = image;
            base.Text = "Untitled - Paint";
            touched = false;
            opened = false;
        }

        private void New_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            if (touched == true)
            {
                DialogResult dr = MessageBox.Show("Do you want to save current work before exiting?", "Caution", MessageBoxButtons.YesNoCancel);
                if (dr == DialogResult.Yes)
                {
                    Save_Click(Save, EventArgs.Empty);
                    if (saved == true)
                        NewPage();
                }
                else
                    if (dr == DialogResult.No)
                    NewPage();
            }
            else
                NewPage();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            OpenFile.Filter = "Bitmap Files (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|PNG (*.png)|*.png|ICO (*.ico)|*.ico|All Files|*.*";
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                if (touched == true)
                {
                    DialogResult dr = MessageBox.Show("Do you want to save current work before exiting?", "Caution", MessageBoxButtons.YesNoCancel);
                    if (dr == DialogResult.Yes)
                    {
                        Save_Click(Save, EventArgs.Empty);
                        if (saved == true)
                        {
                            DesignField.Size = Image.FromFile(OpenFile.FileName).Size;
                            image = new Bitmap(Image.FromFile(OpenFile.FileName));
                            graphic = Graphics.FromImage(image);
                            DesignField.Image = image;
                            opened_image_name = OpenFile.FileName;
                            base.Text = Path.GetFileNameWithoutExtension(OpenFile.FileName) + " - Paint";
                            opened = true;
                            touched = false;
                        }
                    }
                    else if (dr == DialogResult.No)
                    {
                        DesignField.Size = Image.FromFile(OpenFile.FileName).Size;
                        image = new Bitmap(Image.FromFile(OpenFile.FileName));
                        graphic = Graphics.FromImage(image);
                        DesignField.Image = image;
                        opened_image_name = OpenFile.FileName;
                        base.Text = Path.GetFileNameWithoutExtension(OpenFile.FileName) + " - Paint";
                        opened = true;
                        touched = false;
                    }
                }
                else
                {
                    DesignField.Size = Image.FromFile(OpenFile.FileName).Size;
                    image = new Bitmap(Image.FromFile(OpenFile.FileName));
                    graphic = Graphics.FromImage(image);
                    DesignField.Image = image;
                    opened_image_name = OpenFile.FileName;
                    base.Text = Path.GetFileNameWithoutExtension(OpenFile.FileName) + " - Paint";
                    opened = true;
                    touched = false;
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            if (opened == true)
            {
                switch (Path.GetExtension(opened_image_name))
                {
                    case ".jpg":
                        image.Save(opened_image_name, ImageFormat.Jpeg); break;
                    case ".bmp":
                        image.Save(opened_image_name, ImageFormat.Bmp); break;
                    case ".png":
                        image.Save(opened_image_name, ImageFormat.Png); break;
                    case ".gif":
                        image.Save(opened_image_name, ImageFormat.Gif); break;
                    case ".ico":
                        image.Save(opened_image_name, ImageFormat.Icon); break;
                }
                touched = false;
                saved = true;
            }
            else
                SaveAs_Click(SaveAs, e);
        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            SaveFile.Filter = "Bitmap Files (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|GIF (*.gif)|*.gif|PNG (*.png)|*.png|ICO (*.ico)|*.ico";
            SaveFile.FileName = base.Text.Remove(base.Text.Length - 8);
            if (SaveFile.ShowDialog() == DialogResult.OK)
            {
                switch (Path.GetExtension(SaveFile.FileName))
                {
                    case ".jpg":
                        image.Save(SaveFile.FileName, ImageFormat.Jpeg); break;
                    case ".bmp":
                        image.Save(SaveFile.FileName, ImageFormat.Bmp); break;
                    case ".png":
                        image.Save(SaveFile.FileName, ImageFormat.Png); break;
                    case ".gif":
                        image.Save(SaveFile.FileName, ImageFormat.Gif); break;
                    case ".ico":
                        image.Save(SaveFile.FileName, ImageFormat.Icon); break;
                }
                opened_image_name = SaveFile.FileName;
                base.Text = Path.GetFileNameWithoutExtension(SaveFile.FileName) + " - Paint";
                opened = true;
                touched = false;
                saved = true;
            }
            else
                saved = false;
        }

        private void Print_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            if (image.Width > image.Height)
                PrintDoc.DefaultPageSettings.Landscape = true;
            else
                PrintDoc.DefaultPageSettings.Landscape = false;
            PrintPage.Document = PrintDoc;
            if (PrintPage.ShowDialog() == DialogResult.OK)
            {
                if (PrintPage.PrintToFile == true)
                    SaveAs_Click(sender, e);
                else
                    PrintDoc.Print();
            }
        }

        private void PrintPreview_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            if (image.Width > image.Height)
                PrintDoc.DefaultPageSettings.Landscape = true;
            else
                PrintDoc.DefaultPageSettings.Landscape = false;
            PrintPreviewPage.Document = PrintDoc;
            PrintPreviewPage.ShowDialog();
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(image, 0, 0, image.Width, image.Height);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            this.Close();
        }
        #endregion

        #region Page Resizing
        private void PaintProject_Load(object sender, EventArgs e)
        {
            SizeStatus.Text = DesignField.Size.Width + " × " + DesignField.Size.Height + "px";
        }

        Point DesignFieldStartSize;

        private void ChangeSizeButton_MouseDown(object sender, MouseEventArgs e)
        {
            DesignFieldStartSize = e.Location;
            FileMenu_Close(sender, e);
        }

        private void ChangeSizeButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DesignField.Size = new Size(DesignField.Width + (e.X - DesignFieldStartSize.X), DesignField.Height + (e.Y - DesignFieldStartSize.Y));
        }

        private void ChangeHeightButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DesignField.Size = new Size(DesignField.Width, DesignField.Height + (e.Y - DesignFieldStartSize.Y));
        }

        private void ChangeWidthButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DesignField.Size = new Size(DesignField.Width + (e.X - DesignFieldStartSize.X), DesignField.Height);
        }

        private void ChangeSizeButton_MouseUp(object sender, MouseEventArgs e)
        {
            Bitmap bmp = new Bitmap(image);
            image = new Bitmap(DesignField.Width, DesignField.Height);
            graphic = Graphics.FromImage(image);
            graphic.Clear(Color.White);
            graphic.DrawImage(bmp, new Point(0, 0));
            DesignField.Image = image;
        }

        private void DesignField_SizeChanged(object sender, EventArgs e)
        {
            SizeStatus.Text = DesignField.Size.Width + " × " + DesignField.Size.Height + "px";
            ChangeWidthButton.Location = new Point(DesignField.Location.X + DesignField.Width, DesignField.Location.Y + DesignField.Height / 2);
            ChangeHeightButton.Location = new Point(DesignField.Location.X + DesignField.Width / 2, DesignField.Location.Y + DesignField.Height);
            ChangeSizeButton.Location = new Point(DesignField.Location.X + DesignField.Width, DesignField.Location.Y + DesignField.Height);

        }
        #endregion

        #region ToolBox Items
        private void Color1Button_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            if (ColorSelector.ShowDialog() == DialogResult.OK)
                Color1Button.BackColor = ColorSelector.Color;
        }

        private void Color2Button_Click(object sender, EventArgs e)
        {
            FileMenu_Close(sender, e);
            if (ColorSelector.ShowDialog() == DialogResult.OK)
                Color2Button.BackColor = ColorSelector.Color;
        }

        #region ColorBox Items
        private void clr3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr3.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr3.BackColor;
        }

        private void clr5_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr5.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr5.BackColor;
        }

        private void clr7_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr7.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr7.BackColor;
        }

        private void clr1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr1.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr1.BackColor;
        }

        private void clr9_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr9.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr9.BackColor;
        }

        private void clr11_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr11.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr11.BackColor;
        }

        private void clr13_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr13.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr13.BackColor;
        }

        private void clr15_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr15.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr15.BackColor;
        }

        private void clr17_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr17.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr17.BackColor;
        }

        private void clr19_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr19.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr19.BackColor;
        }

        private void clr21_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr21.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr21.BackColor;
        }

        private void clr23_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr23.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr23.BackColor;
        }

        private void clr2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr2.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr2.BackColor;
        }

        private void clr4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr4.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr4.BackColor;
        }

        private void clr6_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr6.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr6.BackColor;
        }

        private void clr8_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr8.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr8.BackColor;
        }

        private void clr10_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr10.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr10.BackColor;
        }

        private void clr12_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr12.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr12.BackColor;
        }

        private void clr14_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr14.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr14.BackColor;
        }

        private void clr16_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr16.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr16.BackColor;
        }

        private void clr18_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr18.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr18.BackColor;
        }

        private void clr20_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr20.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr20.BackColor;
        }

        private void clr22_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr22.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr22.BackColor;
        }

        private void clr24_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Color1Button.BackColor = clr24.BackColor;
            if (e.Button == MouseButtons.Right)
                Color2Button.BackColor = clr24.BackColor;
        }
        #endregion

        private void PencilButton_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Pencil;
            ShowSelectedAction.Text = "Pencil";
            FileMenu_Close(sender, e);
        }

        private void SelectLine_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Line;
            ShowSelectedAction.Text = "Draw Line";
            FileMenu_Close(sender, e);
        }

        private void SelectEllipse_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Ellipse;
            ShowSelectedAction.Text = "Draw Ellipse";
            FileMenu_Close(sender, e);
        }

        private void SelectRectangle_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Rectangle;
            ShowSelectedAction.Text = "Draw Rectangle";
            FileMenu_Close(sender, e);
        }

        private void EraserButton_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Eraser;
            ShowSelectedAction.Text = "Eraser";
            FileMenu_Close(sender, e);
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Select;
            ShowSelectedAction.Text = "Selection";
            FileMenu_Close(sender, e);
        }

        private void BucketButton_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Bucket;
            ShowSelectedAction.Text = "Bucket";
            FileMenu_Close(sender, e);
        }

        private void SizeButton_Click(object sender, EventArgs e)
        {
            if (SizeMenu.Visible == false)
            {
                SizeMenu.Visible = true;
                FileMenu.Visible = false;
                FileButton.BackColor = Color.Blue;
            }
            else
                FileMenu_Close(sender, e);
        }

        private void TextButton_Click(object sender, EventArgs e)
        {
            selectedAction = Choice.Text;
            ShowSelectedAction.Text = "Inset Text";
            FileMenu_Close(sender, e);
        }

        #region Pen Sizes
        private void Button1px_Click(object sender, EventArgs e)
        {
            penWidth = 1;
            FileMenu_Close(sender, e);
        }

        private void Button3px_Click(object sender, EventArgs e)
        {
            penWidth = 3;
            FileMenu_Close(sender, e);
        }

        private void Button5px_Click(object sender, EventArgs e)
        {
            penWidth = 5;
            FileMenu_Close(sender, e);
        }

        private void Button8px_Click(object sender, EventArgs e)
        {
            penWidth = 8;
            FileMenu_Close(sender, e);
        }
        #endregion
        #endregion

        #region Painting
        private void DesignField_MouseMove(object sender, MouseEventArgs e)
        {
            PositionStatus.Text = e.X + ", " + e.Y + "px";
            width = Math.Abs(e.X - startPoint.X);
            height = Math.Abs(e.Y - startPoint.Y);
            sp = new Point((startPoint.X < e.X) ? startPoint.X : e.X, (startPoint.Y < e.Y) ? startPoint.Y : e.Y);
            Bitmap keep;
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                switch (selectedAction)
                {
                    case Choice.Rectangle:
                        keep = new Bitmap(image);
                        using (Graphics g = Graphics.FromImage(keep))
                            g.DrawRectangle(pen, sp.X, sp.Y, width, height);
                        DesignField.Image = keep;
                        break;
                    case Choice.Ellipse:
                        keep = new Bitmap(image);
                        using (Graphics g = Graphics.FromImage(keep))
                            g.DrawEllipse(pen, sp.X, sp.Y, width, height);
                        DesignField.Image = keep;
                        break;
                    case Choice.Line:
                        keep = new Bitmap(image);
                        using (Graphics g = Graphics.FromImage(keep))
                            g.DrawLine(pen, startPoint, e.Location);
                        DesignField.Image = keep;
                        break;
                    case Choice.Pencil:
                    case Choice.Eraser:
                        graphic.DrawEllipse(new Pen(pen.Color, pen.Width / 2), e.X - (pen.Width / 3), e.Y - (pen.Width / 3), pen.Width / 2, pen.Width / 2);
                        graphic.DrawLine(pen, startPoint, e.Location);
                        startPoint = e.Location;
                        DesignField.Image = image;
                        break;
                    case Choice.Select:
                        DesignField.Refresh();
                        float[] dashValues = { 5, 5 };
                        pen = new Pen(Color.Blue, 1);
                        pen.DashPattern = dashValues;
                        keep = new Bitmap(image);
                        using (Graphics g = Graphics.FromImage(keep))
                            g.DrawRectangle(pen, sp.X, sp.Y, width, height);
                        DesignField.Image = keep;
                        break;
                }
            }
        }

        private void DesignField_MouseDown(object sender, MouseEventArgs e)
        {
            FileMenu_Close(sender, e);
            touched = true;
            if (e.Button == MouseButtons.Left)
                selectedColor = Color1Button.BackColor;
            if (e.Button == MouseButtons.Right || selectedAction == Choice.Eraser)
                selectedColor = Color2Button.BackColor;
            startPoint = e.Location;
            pen = new Pen(selectedColor, penWidth);
            if (pb != null)
            {
                graphic.DrawImage(pb.Image, pb.Location);
                DesignField.Image = image;
                DesignField.Controls.Remove(pb);
                pb = null;
            }
            if (selectedAction == Choice.Bucket)
            {
                if (image.GetPixel(e.X, e.Y).ToArgb() != selectedColor.ToArgb())
                    FloodFill(image, e.Location, image.GetPixel(e.X, e.Y), selectedColor);
            }
        }

        private void DesignField_MouseUp(object sender, MouseEventArgs e)
        {
            width = Math.Abs(e.X - startPoint.X);
            height = Math.Abs(e.Y - startPoint.Y);
            sp = new Point((startPoint.X < e.X) ? startPoint.X : e.X, (startPoint.Y < e.Y) ? startPoint.Y : e.Y);
            temp = new Bitmap(width + 25, height + 25);
            switch (selectedAction)
            {
                case Choice.Rectangle:
                    DesignField.Image = image;
                    using (Graphics g = Graphics.FromImage(temp))
                        g.DrawRectangle(pen, 10, 10, width, height);
                    Select_Frame();
                    break;
                case Choice.Ellipse:
                    DesignField.Image = image;
                    using (Graphics g = Graphics.FromImage(temp))
                        g.DrawEllipse(pen, 10, 10, width, height);
                    Select_Frame();
                    break;
                case Choice.Line:
                    graphic.DrawLine(pen, startPoint, e.Location);
                    DesignField.Image = image;
                    break;
                case Choice.Pencil:
                case Choice.Eraser:
                    graphic.DrawEllipse(new Pen(pen.Color, pen.Width / 2), e.X - (pen.Width / 3), e.Y - (pen.Width / 3), pen.Width / 2, pen.Width / 2);
                    DesignField.Image = image;
                    break;
                case Choice.Select:
                    using (Graphics g = Graphics.FromImage(temp))
                        g.DrawImage(image, new Point(-sp.X, -sp.Y));
                    Select_Frame();
                    if (width > 0 && height > 0)
                    {
                        Rectangle ra = new Rectangle(pb.Location, pb.Size);
                        graphic.FillRectangle(new SolidBrush(Color.White), ra);
                        DesignField.Image = image;
                    }
                    break;
            }
        }
        #endregion

        private void DesignField_MouseLeave(object sender, EventArgs e)
        {
            PositionStatus.Text = "";
        }

        private void PaintProject_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (touched == true)
            {
                var dr = MessageBox.Show("Do you want to save current work before exiting?", "Caution",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    Save_Click(Save, EventArgs.Empty);
                    if (saved == false)
                        e.Cancel = true;
                }
                else if (dr == DialogResult.Cancel)
                    e.Cancel = true;
            }
        }

        private static bool ColorMatch(Color a, Color b)
        {
            return (a.ToArgb() & 0xffffff) == (b.ToArgb() & 0xffffff);
        }

        private void FloodFill(Bitmap bmp, Point pt, Color targetColor, Color replacementColor)
        {
            Queue<Point> q = new Queue<Point>();
            q.Enqueue(pt);
            while (q.Count > 0)
            {
                Point n = q.Dequeue();
                if (ColorMatch(bmp.GetPixel(n.X, n.Y), targetColor) == false)
                    continue;
                Point w = n, e = new Point(n.X + 1, n.Y);
                while ((w.X > 0) && ColorMatch(bmp.GetPixel(w.X, w.Y), targetColor))
                {
                    bmp.SetPixel(w.X, w.Y, replacementColor);
                    if ((w.Y > 0) && ColorMatch(bmp.GetPixel(w.X, w.Y - 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y - 1));
                    if ((w.Y < bmp.Height - 1) && ColorMatch(bmp.GetPixel(w.X, w.Y + 1), targetColor))
                        q.Enqueue(new Point(w.X, w.Y + 1));
                    w.X--;
                }
                while ((e.X < bmp.Width - 1) && ColorMatch(bmp.GetPixel(e.X, e.Y), targetColor))
                {
                    bmp.SetPixel(e.X, e.Y, replacementColor);
                    if ((e.Y > 0) && ColorMatch(bmp.GetPixel(e.X, e.Y - 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y - 1));
                    if ((e.Y < bmp.Height - 1) && ColorMatch(bmp.GetPixel(e.X, e.Y + 1), targetColor))
                        q.Enqueue(new Point(e.X, e.Y + 1));
                    e.X++;
                }
            }
            DesignField.Image = image;
        }

        private void FileMenu_Close(object sender, EventArgs e)
        {
            FileMenu.Visible = false;
            SizeMenu.Visible = false;
            FileButton.BackColor = Color.Blue;
        }

        private void pb_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
        }

        private void pb_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                pb.Location = new Point(pb.Location.X + (e.X - startPoint.X), pb.Location.Y + (e.Y - startPoint.Y));
        }

        void Select_Frame()
        {
            if (width > 0 && height > 0)
            {
                pb = new PictureBox();
                pb.Size = new Size(width + 25, height + 25);
                pb.Image = temp;
                pb.Location = sp;
                pb.Cursor = Cursors.SizeAll;
                pb.BorderStyle = BorderStyle.FixedSingle;
                pb.MouseDown += pb_MouseDown;
                pb.MouseMove += pb_MouseMove;
                DesignField.Controls.Add(pb);
            }
        }

        private void DesignField_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectedAction == Choice.Text)
            {
                tb.Location = e.Location;
                tb.KeyPress += tb_KeyPress;
                DesignField.Controls.Add(tb);
            }
        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                graphic.DrawString(tb.Text, new Font(new FontFamily("Arial"), penWidth + 10), new SolidBrush(Color1Button.BackColor), tb.Location);
                DesignField.Image = image;
                DesignField.Controls.Remove(tb);
                tb.Text = "";
            }
            if (e.KeyChar == (char)Keys.Escape)
                DesignField.Controls.Remove(tb);
        }
    }
}