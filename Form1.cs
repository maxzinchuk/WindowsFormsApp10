using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp10
{
    public partial class Form1 : Form
    {
        public static List<Snake> snakes = new List<Snake>();
        List<Path>   food  =  new List<Path>();
        public Form1()
        {
            InitializeComponent();
            List<Path> newSnake = new List<Path>();
            newSnake.Add(new Path(50, 50));
            newSnake.Add(new Path(51, 50));
            newSnake.Add(new Path(52, 50));
            snakes.Add(new Snake(newSnake));
            for (int i = 0; i < 300; i++)
            {
                food.Add(new Path(r.Next(100), r.Next(100)));
            }
            for (int i = 0; i < snakes.Count; i++)
            {
                for (int j = 0; j < snakes[i].Parts.Count; j++)
                {
                    Image.SetPixel(snakes[i].Parts[j].X, snakes[i].Parts[j].Y, Color.Green);
                }
            }
            Draw();
           
        }
        class Trigger {
            bool Type;
            int[] weight;
            public Trigger(bool FoodSnake) {
                Type = FoodSnake;
                Weight = new int[4];
            }

            

            public int[] Weight { get => weight; set => weight = value; }
            public void TeachTrig(int x, int y, int valid)
            {
                if (!Type)
                {
                    if (Image.GetPixel(x, y).R == 255)
                    {
                        weight[valid] += 2;
                        for (int i = 0; i < 4; i++)
                        {
                            weight[i]--;
                        }
                        return;
                    }
                    return;
                }
                else
                {
                    if (Image.GetPixel(x, y).G == Image.GetPixel(snakes[0].Parts[1].X, snakes[0].Parts[1].Y).G)
                    {
                        weight[valid] += 2;
                        for (int i = 0; i < 4; i++)
                        {
                            weight[i]--;
                        }
                        return;
                    }
                    return;
                }
            }

            public int[] Trig(int x,int y) {
                if (!Type)
                {
                    if (Image.GetPixel(x, y).R == 255) {
                        return Weight;
                    }
                    return new int[4];
                }
                else {
                    if (Image.GetPixel(x, y).G == Image.GetPixel(snakes[0].Parts[1].X, snakes[0].Parts[1].Y).G)
                    {
                        return Weight;
                    }
                    return new int[4];
                }
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        public class Path{
            int x, y;
            public Path(int xPos, int yPos) {
                X = xPos;
                Y = yPos;
            }

            public int X { get => x; set => x = value; }
            public int Y { get => y; set => y = value; }
        }
        public static Bitmap Image = new Bitmap(100,100);
        public class Snake
        {
            int energy = 16;
            List<Path> parts = new List<Path>();
            List<Trigger> bloksTrig = new List<Trigger>();
            List<Trigger> foodTrig = new List<Trigger>();
            const int size = 21;
            float Srotation = 1;
            float Soldrotation = 1;
            public Snake(List<Path> snake) {
                Parts = snake;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        bloksTrig.Add(new Trigger(true));
                         foodTrig.Add(new Trigger(true));
                    }
                }
            }
            public Snake(List<Path> snake,float rotation)
            {
                Parts = snake;
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        bloksTrig.Add(new Trigger(true));
                         foodTrig.Add(new Trigger(false));
                    }
                }
                Srotation = rotation;
            }
            public void Teach(int valid) {
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        bloksTrig[i].TeachTrig(i - (size / 2) +1, j - (size / 2) + 1, valid);
                        foodTrig[i].TeachTrig (i - (size / 2) +1, j - (size / 2) + 1, valid);
                    }
                }
            }
            public void rotate(int newRotate) {
                Soldrotation = newRotate;
            }
            public void Think() {
                int[] Weights = new int[4];
                int[] WeightsTrig = new int[4];
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        WeightsTrig = bloksTrig[i].Trig(i - (size / 2), j - (size / 2));
                        for (int w = 0; w < 4; w++)
                        {
                            Weights[w] += WeightsTrig[w];
                        }
                        WeightsTrig = foodTrig[i].Trig(i - (size / 2), j - (size / 2));
                        for (int w = 0; w < 4; w++)
                        {
                            Weights[w] += WeightsTrig[w];
                        }
                    }
                }
                int B = 0;
                for (int w = 0; w < 4; w++)
                {
                    if (B < Weights[w]) {
                        B = Weights[w];
                        Srotation = w;
                    }
                }
                if (Math.Abs(Soldrotation - Srotation) == 2)
                {
                    Srotation = Soldrotation;
                }
                switch (Srotation) {
                    case 0:

                        Parts.Insert(0, new Path(Parts[0].X, Parts[0].Y - 1));
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y).R != 255)
                        {
                            Image.SetPixel(Parts[Parts.Count - 1].X, Parts[Parts.Count - 1].Y, Color.Empty);
                            Parts.RemoveAt(Parts.Count - 1);

                        }
                        else energy = 16;
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y) == Image.GetPixel(Parts[1].X, Parts[1].Y))
                        {
                            energy = 1000;
                        }
                        break;
                    case 2:
                        Parts.Insert(0, new Path(Parts[0].X, Parts[0].Y + 1));
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y).R != 255)
                        {
                            Image.SetPixel(Parts[Parts.Count - 1].X, Parts[Parts.Count - 1].Y, Color.Empty);
                            Parts.RemoveAt(Parts.Count - 1);
                        }
                        else energy = 16;
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y) == Color.Green)
                        {
                            energy = 1000;
                        }
                        break;

                    case 1:
                        Parts.Insert(0, new Path(Parts[0].X - 1, Parts[0].Y));
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y).R != 255)
                        {
                            Image.SetPixel(Parts[Parts.Count - 1].X, Parts[Parts.Count - 1].Y, Color.Empty);
                            Parts.RemoveAt(Parts.Count - 1);
                        }
                        else energy = 16;
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y) == Color.Green)
                        {
                            energy = 1000;
                        }
                        break;
                    case 3:
                        Parts.Insert(0, new Path(Parts[0].X + 1, Parts[0].Y));
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y).R != 255)
                        {
                            Image.SetPixel(Parts[Parts.Count - 1].X, Parts[Parts.Count - 1].Y, Color.Empty);
                            Parts.RemoveAt(Parts.Count - 1);
                        }
                        else energy = 16;
                        if (Image.GetPixel(Parts[0].X, Parts[0].Y) == Color.Green)
                        {
                            energy = 1000;
                        }
                        break;
                }
            }

            public List<Path> Parts { get => parts; set => parts = value; }
            public int Energy { get => energy; set => energy = value; }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Form1_KeyDown(null, new KeyEventArgs(Keys.F24));
            Draw();
        }
        private void pictureBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }
        int rotation = 1;
        int oldrotation = 1;
        Random r = new Random();
        public void Draw() {
            for (int i = 0; i < snakes.Count; i++)
            {
                for (int j = 0; j < snakes[i].Parts.Count; j++)
                {
                    Image.SetPixel(snakes[i].Parts[j].X, snakes[i].Parts[j].Y, Color.Green);
                }
            }
            for (int j = 0; j < food.Count; j++)
            {
                Image.SetPixel(food[j].X, food[j].Y, Color.Red);
                food.RemoveAt(j);
            }
            pictureBox1.Image = Image;

        }
        bool Play=true;
        int Tick = 10;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Tick--;
            if (Tick == 0)
            {
                Tick = 10;
                food.Add(new Path(r.Next(100), r.Next(100)));
            }
            try
                {
                    Keys key = e.KeyCode;
                switch (key)
                {
                    case Keys.W: rotation = 0; break;
                    case Keys.A: rotation = 1; break;
                    case Keys.S: rotation = 2; break;
                    case Keys.D: rotation = 3; break;
                    default:
                        break;
                }
                if (Math.Abs(oldrotation - rotation) == 2)
                {
                    rotation = oldrotation;
                }
                oldrotation = rotation;
                label1.Text = snakes[0].Energy + "";
                for (int i = 0; i < snakes.Count; i++)
                {
                    snakes[i].Energy--;
                    if (snakes[i].Energy < 1 || snakes[i].Energy > 16)
                    {
                        snakes[i].Energy = 16 + snakes[i].Energy;
                        Image.SetPixel(snakes[i].Parts[snakes[i].Parts.Count - 1].X, snakes[i].Parts[snakes[i].Parts.Count - 1].Y, Color.Empty);
                        snakes[i].Parts.RemoveAt(snakes[i].Parts.Count - 1);
                    }
                }
                //snakes[0].Teach(rotation);
                bool EatenFood;
                
                    switch (rotation)
                    {
                        case 0:

                            snakes[0].Parts.Insert(0, new Path(snakes[0].Parts[0].X, snakes[0].Parts[0].Y - 1));
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y).R != 255)
                            {
                                Image.SetPixel(snakes[0].Parts[snakes[0].Parts.Count - 1].X, snakes[0].Parts[snakes[0].Parts.Count - 1].Y, Color.Empty);
                                snakes[0].Parts.RemoveAt(snakes[0].Parts.Count - 1);

                            }
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y) == Image.GetPixel(snakes[0].Parts[1].X, snakes[0].Parts[1].Y))
                            {
                                snakes[0].Energy += 1000;
                            }
                            Image.SetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y, Color.Green);

                            break;
                        case 2:
                            snakes[0].Parts.Insert(0, new Path(snakes[0].Parts[0].X, snakes[0].Parts[0].Y + 1));
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y).R != 255)
                            {
                                Image.SetPixel(snakes[0].Parts[snakes[0].Parts.Count - 1].X, snakes[0].Parts[snakes[0].Parts.Count - 1].Y, Color.Empty);
                                snakes[0].Parts.RemoveAt(snakes[0].Parts.Count - 1);
                            }
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y) == Image.GetPixel(snakes[0].Parts[1].X, snakes[0].Parts[1].Y))
                            {
                                snakes[0].Energy += 1000;
                            }
                            break;

                        case 1:
                            snakes[0].Parts.Insert(0, new Path(snakes[0].Parts[0].X - 1, snakes[0].Parts[0].Y));
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y).R != 255)
                            {
                                Image.SetPixel(snakes[0].Parts[snakes[0].Parts.Count - 1].X, snakes[0].Parts[snakes[0].Parts.Count - 1].Y, Color.Empty);
                                snakes[0].Parts.RemoveAt(snakes[0].Parts.Count - 1);
                            }
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y) == Image.GetPixel(snakes[0].Parts[1].X, snakes[0].Parts[1].Y))
                            {
                                snakes[0].Energy += 1000;
                            }
                            break;
                        case 3:
                            snakes[0].Parts.Insert(0, new Path(snakes[0].Parts[0].X + 1, snakes[0].Parts[0].Y));
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y).R != 255)
                            {
                                Image.SetPixel(snakes[0].Parts[snakes[0].Parts.Count - 1].X, snakes[0].Parts[snakes[0].Parts.Count - 1].Y, Color.Empty);
                                snakes[0].Parts.RemoveAt(snakes[0].Parts.Count - 1);
                            }
                            if (Image.GetPixel(snakes[0].Parts[0].X, snakes[0].Parts[0].Y) == Image.GetPixel(snakes[0].Parts[1].X, snakes[0].Parts[1].Y))
                            {
                                snakes[0].Energy += 1000;
                            }
                            break;
                    }
                    
                    Draw();
                    
                }
                catch {
                    try
                    {
                        snakes.Remove(snakes[0]);
                    }
                    catch { 
                    
                    }
                    Draw();

                }
            
        }
    }
}
