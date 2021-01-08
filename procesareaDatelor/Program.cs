using System;
using System.Collections.Generic;
using System.Text;
namespace procesareaDatelor
{
    class RGB {
        int red;
        int blue;
        int green;
        public RGB(int r, int g, int b) {
            Red = r;
            Blue = b;
            Green = g;
        }
        public int Red { get => red; set => red = value; }
        public int Blue { get => blue; set => blue = value; }
        public int Green { get => green; set => green = value; }
    }

    class YUV
    {
        double lum;
        double blue;
        double red;
        public YUV(double y, double u, double v)
        {
            Y = y;
            U = u;
            V = v;
        }
        public double V { get => red; set => red = value; }
        public double U { get => blue; set => blue = value; }
        public double Y { get => lum; set => lum = value; }
    }
    class Block {

        private int size;
        private char type;
        private List<List<double>> blocks;
        public Block(int s, char t, List<List<double>> b) {
            Size = s;
            Type = t;
            Blocks = new List<List<double>> ();
            foreach (List<double> list in b){
                Blocks.Add(new List<double>());
                foreach (double value in list)
                {
                    Blocks[Blocks.Count-1].Add(value);
                }
            }
        }
       public void subtractAll(int x) {
            for (int i = 0; i < size; i++) {
                for (int j = 0; j < size; j++) {
                    Blocks[i][j] = Blocks[i][j]- x;
                }
            }
        }
        public void addAll(int x)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Blocks[i][j] = Blocks[i][j] + x;
                }
            }
        }
        public void DTCtransformation() {
            List<List<double>> oldBlocks = Blocks;
            Blocks = new List<List<double>>();
            
            for (int u = 0; u < size; u++) {
                Blocks.Add(new List<double> { 0,0,0,0,0,0,0,0});
                for (int v = 0; v < size; v++) {
                    double doubleSum = 0;
                    for (int x = 0; x < size; x++) { 
                        for (int y = 0; y < size; y++)
                        
                            doubleSum += oldBlocks[x][y] * Math.Cos(((2 * x + 1) * Math.PI * u )/ 16) * Math.Cos(((2 * y + 1) * Math.PI * v) / 16);
                        
                    }
                    Blocks[u][v] =  Program.alpha(u) * Program.alpha(v) * doubleSum/4;
                }
            
            }
        }
        public void IDTCtransformation()
        {
            List<List<double>> oldBlocks = Blocks;
            Blocks = new List<List<double>>();
            for (int i = 0; i < size; i++)
            {
                Blocks.Add(new List<double> { 0, 0, 0, 0, 0, 0, 0, 0 });
                for (int j = 0; j < size; j++)
                {
                    double doubleSum = 0;
                    for (int k = 0; k < size; k++)
                    {
                        for (int l = 0; l < size; l++)
                        {
                            doubleSum += Program.alpha(k)*Program.alpha(l)*oldBlocks[k][l] * Math.Cos(((2 * i + 1) * Math.PI * k) / 16) * Math.Cos(((2 * j + 1) * Math.PI * l )/ 16);
                        }
                    }
                    Blocks[i][j] =  doubleSum/4;
                }

            }
        }
        public void Quantization(Block quantMatrix) {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Blocks[i][j] = Math.Round( Blocks[i][j] / quantMatrix.Blocks[i][j]); 
                }
            }
        }
        public void DEQuantization(Block quantMatrix)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Blocks[i][j] *= quantMatrix.Blocks[i][j];
                }
            }
        }
        public void subsampling(int x) {
            List<List<double>> newBlocks = new List<List<double>>(); 

            for (int i = 0; i < size; i += x)
            {

                newBlocks.Add(new List<double>());
                for (int j = 0; j < size; j += x)
                {

                    double sampleSum = 0;
                    for (int k = i; k < i+x; k++)
                    {
                        for (int l = j; l < j+x; l++)
                        {
                            sampleSum += Blocks[k][l];

                        }
                    }
                    newBlocks[i / x].Add(Math.Round(sampleSum / (x*x)));
                }
            }

            Size /= x;
            Blocks = newBlocks;
        }
        public void oversampling(int x)
        {
            List<List<double>> newBlocks = new List<List<double>>();

            for (int i = 0; i < size; i ++)
            {
                for (int o=0;o<x;o++)newBlocks.Add(new List<double>());
                for (int j = 0; j < size; j ++)
                {
                    for (int o = 0; o < x; o++)
                    {
                        for (int p = 0; p < x; p++)
                        {
                            newBlocks[i*x+o].Add(Blocks[i][j]);
                        }
                    }
                }
            }

            Size *= x;
            Blocks = newBlocks;
        }

        public int Size { get => size; set => size = value; }
        public char Type { get => type; set => type = value; }
        public List<List<double>> Blocks { get => blocks; set => blocks = value; }
        public override string ToString()
        {
            string result = "type " + type + " block with " + size * size + " pixels:";
            for (int i = 0; i < Blocks.Count; i++) {
                result += "\n";
                for (int j = 0; j < Blocks[i].Count; j++) {
                    result += Blocks[i][j] + " ";
                }
            }
            return result; 
        }
        public List<int> zigZag() {
            List<int> result = new List<int>();
            List<int> right = new List<int> { 0, 1 };
            List<int> down = new List<int> { 1, 0 };
            List<int> crossDown = new List<int> { 1, -1 };
            List<int> crossUp = new List<int> { -1, 1 };
 
            int i = 0,j=0;
            result.Add((int)Blocks[i][j]);
            i += right[0];
            j += right[1];
            while (i!=size-1 || j != size - 1)
            {
                //crossdown
                while (j != 0 && i != size - 1) {
                    result.Add((int)Blocks[i][j]);
                    i += crossDown[0];
                    j += crossDown[1];
                }
                result.Add((int)Blocks[i][j]);
                //down or right
                if (i != size - 1)
                {
                    i += down[0];
                    j += down[1];
                }
                else {
                    i += right[0];
                    j += right[1];
                }
                if (i == size - 1 && j == size - 1) continue;
                //cross up
                while (j != size-1 && i != 0)
                {
                    result.Add((int)Blocks[i][j]);
                    i += crossUp[0];
                    j += crossUp[1];
                }
                result.Add((int)Blocks[i][j]);
                //right or down
                if (j != size-1)
                {
                    i += right[0];
                    j += right[1];
                }
                else
                {
                    i += down[0];
                    j += down[1];
                }

            }
            result.Add((int)Blocks[i][j]);

            return result;
        
        }

        public static Block reverseZigZag(List<int> source)
        {
            List<List<double>> arg = new List<List<double>>();

            for (int o = 0; o <= 7; o++) {
                arg.Add(new List<double> { 0, 0, 0, 0, 0, 0, 0, 0 });
            }
            int size = 8;
            List<int> result = new List<int>();
            List<int> right = new List<int> { 0, 1 };
            List<int> down = new List<int> { 1, 0 };
            List<int> crossDown = new List<int> { 1, -1 };
            List<int> crossUp = new List<int> { -1, 1 };
            int index = 0;
            int i = 0, j = 0;
            arg[i][j] = source[index++];
            i += right[0];
            j += right[1];
            
            while (i != size - 1 || j != size - 1)
            {
                //crossdown
                while (j != 0 && i != size - 1)
                {
                    arg[i][j] = source[index++];
                    i += crossDown[0];
                    j += crossDown[1];
                }
                arg[i][j] = source[index++];
                //down or right
                if (i != size - 1)
                {
                    i += down[0];
                    j += down[1];
                }
                else
                {
                    i += right[0];
                    j += right[1];
                }
                if (i == size - 1 && j == size - 1) continue;
                //cross up
                while (j != size - 1 && i != 0)
                {
                    arg[i][j] = source[index++];
                    i += crossUp[0];
                    j += crossUp[1];
                }
                arg[i][j] = source[index++];
                //right or down
                if (j != size - 1)
                {
                    i += right[0];
                    j += right[1];
                }
                else
                {
                    i += down[0];
                    j += down[1];
                }
            }
            arg[i][j] = source[index++];
            return new Block(8,'a',arg);
        }
        public static List<sbyte> encodedZigZag(List<int> zigzag) {
            List<sbyte> result = new List<sbyte>();
            int dc = 0;
            int size = 0;
       
            if (zigzag[0] < 0)
            {
                dc = -zigzag[0];
                size = 64;
            }
            else {
                dc = zigzag[0];
            }
            int copydc = dc;
           
            int zeroCount = 0;
            while ( copydc != 0) {
                size+=1;
                copydc /= 2;
            }
           
            result.Add((sbyte)size);
            result.Add((sbyte)dc);
            for (int i = 1; i < zigzag.Count; i++) {
                dc = Program.signedClamp(zigzag[i]);
                if (dc == 0) {
                    zeroCount += 1;
                    continue;
                }
                size = 0;
                copydc = dc;
                while (copydc != 0)
                {
                    size++;
                    copydc /= 2;
                }
                result.Add((sbyte)zeroCount);
                result.Add((sbyte)size);
                result.Add((sbyte)dc);
                zeroCount = 0;
            }
            if (zeroCount > 0) {
                result.Add((sbyte)0);
                result.Add((sbyte)0);
            }
            return result;
        }

        public static List<int> decodeZigZag(List<sbyte> encodedZigZag) {
            List<int> result = new List<int>();
            int i = 0;
            int rest = 63;
            while (i < encodedZigZag.Count) {
                
                int x = (byte)encodedZigZag[i + 1];
                if (encodedZigZag[i] > 63)
                {
                    result.Add(-x);
                }
                else {
                    result.Add(x);
                }
                i += 2;
                rest = 63;
                for (;rest!=0;i=i+3) {
                    int first = encodedZigZag[i];
                    int second = encodedZigZag[i+1];
                    int third=0;
                    if (i +2 < encodedZigZag.Count) { third = encodedZigZag[i + 2]; }
                    for (int k = 0; k < first; k++) {
                        result.Add(0);
                        rest -= 1;
                    }
                    if (second != 0)
                    {
                        result.Add(third);
                        rest -= 1;
                    }
                    else {
                        i -= 1;
                        for (; rest > 0; rest--) {
                            result.Add(0);
                        }
                    }
                }
            }
            return result;
        }

        public static List<List<Block>> fromDecodingToBlocks(List<int> decodedZigZag) {
            List<List<Block>> result = new List<List<Block>>();
            List<Block> yB = new List<Block>();
            List<Block> uB = new List<Block>();
            List<Block> vB = new List<Block>();
            int i = 0;
            while (i < decodedZigZag.Count) {

                for (int j = 1; j <= 3; j++) {
                    int next_i = i + 63;
                    List<int> blockEncoding = new List<int>();
                    for (; i <= next_i; i++) {
                        blockEncoding.Add(decodedZigZag[i]);
                    }
                    if (j == 1)
                    {
                        yB.Add(Block.reverseZigZag(blockEncoding));
                    }
                    else if (j == 2)
                    {
                        uB.Add(Block.reverseZigZag(blockEncoding));
                    }
                    else {
                        vB.Add(Block.reverseZigZag(blockEncoding));
                    }
                
                }
            
            }

            result.Add(yB);
            result.Add(uB);
            result.Add(vB);
            return result;
        
        } 


        public static String printByteArray(List<byte> list) {

            StringBuilder result = new StringBuilder();
            result.Append("(");
            result.Append(list[0]);
            result.Append(")");
            result.Append("(");
            result.Append((sbyte)list[1]);
            result.Append(")");
            result.Append(",");
            for (int i = 2; i < list.Count; i=i+3) {


                result.Append("(");
                result.Append((byte)list[i]);
                result.Append(",");
                result.Append((byte)list[i + 1]);
                result.Append(")");
                result.Append("(");
                result.Append((sbyte)list[i + 2]);
                result.Append(")");
                result.Append(",");

            }

            return result.ToString();
        }

    }


    class Program
    {

        public static YUV toYUV(int R, int G, int B) {
            double Y = clamp((int)(0.299 * R + 0.587 * G + 0.114 * B));
            double Cb = clamp((int)(128 - (0.1687 * R) - 0.3312 * G + 0.5 * B));
            double Cr = clamp((int)(128 + 0.5 * R - 0.4186 * G - 0.0813 * B));

            return new YUV(Y, Cb, Cr);
        }
        public static YUV toYUV2(int R, int G, int B)
        {
            double Y = clamp((int)(0.299 * R + 0.587 * G + 0.114 * B));
            double Cb = clamp((int)((-0.14713 * R) - 0.28886 * G + 0.436 * B));
            double Cr = clamp((int)(0.615 * R - 0.5149 * G - 0.10001 * B));

            return new YUV(Y, Cb, Cr);
        }

        public static int clamp(int x)
        {
            if (x > 255) return 255;
            if (x < 0) return 0;
            return x;
        }

        public static int signedClamp(int x)
        {
            if (x > 127) return 127;
            if (x < -128) return -128;
            return x;
        }

        public static double clamp(double x)
        {
            if (x > 255) return 255;
            if (x < 0) return 0;
            return x;
        }
        public static double alpha(int x)
        {
            if (x == 0) return 1.0 / Math.Sqrt(2);
            return 1;
        }
        public static double alpha(double x)
        {
            if (x == 0) return 1.0 / Math.Sqrt(2);
            return 1;
        }

        public static RGB toRGB(double Y, double U, double V) {

            int R = clamp((int)(Y + 1.402 * (V - 128)));
            int G = clamp((int)(Y - 0.344136 * (U - 128) - 0.714136 * (V - 128)));
            int B = clamp((int)(Y + 1.772 * (U - 128)));
            return new RGB(R, G, B);

        }
        public static RGB toRGB2(double Y, double U, double V) {

            int R = clamp((int)(Y + 1.13983 * V));
            int G = clamp((int)(Y - 0.39465 * U - 0.5806 * V));
            int B = clamp((int)(Y + 2.03211 * U));
            return new RGB(R, G, B);

        }

        public static String printList(List<int> list) {

            StringBuilder result = new StringBuilder();
            foreach (int x in list) {
                result.Append(x);
            }
            return result.ToString();
        
        }
        public static Block quantifier = new Block(8, 'q', new List<List<double>>
        {
            new List<double>{ 6,4,4,6,10,16,20,24 },
            new List<double>{ 5,5,6,8,10,23,24,22 },
            new List<double>{ 6,5,6,10,16,23,28,22},
            new List<double>{ 6,7,9,12,20,35,32,25},
            new List<double>{ 7,9,15,22,27,44,41,31},
            new List<double>{ 10,14,22,26,32,42,45,37},
            new List<double>{ 20,26,31,35,41,48,48,40},
            new List<double>{ 29,37,38,39,45,40,41,40}
        });

        static void Main(string[] args)
        {
           //encoder
            string[] lines = System.IO.File.ReadAllLines("nt-P3.ppm");
            string size = lines[2];
            
            
            string[] wh = size.Split(' ');
            int width = Int32.Parse(wh[0]);
            int height = Int32.Parse(wh[1]);
            int maxValue = Int32.Parse(lines[3]);
            double[][] Y = new double[height][];
            double[][] U = new double[height][];
            double[][] V = new double[height][];
            int r, b, g;
            Y[0] = new double[width]; 
            U[0] = new double[width]; 
            V[0] = new double[width];
            List<Block> yBlocks = new List<Block>();
            List <Block> uBlocks = new List<Block>();
            List<Block> vBlocks = new List<Block>();

            int line = 0;
            int col = 0;
            for (int i=4; i<lines.Length; i +=3)
            {
                r = Int32.Parse(lines[i]);
                g = Int32.Parse(lines[i+1]);
                b = Int32.Parse(lines[i+2]);

                YUV color = toYUV(r,g,b);
                

                Y[line][col] = color.Y;
                U[line][col] = color.U;
                V[line][col] = color.V;

                col += 1;
                if (col == width)
                {
                    col = 0;
                    line += 1;
                    if (line < height)
                    {
                        Y[line] = new double[width];
                        U[line] = new double[width];
                        V[line] = new double[width];
                    }
                }
            }



            List<List<double>> tempMatrix;
            List<List<double>> tempMatrix2;
            List<List<double>> tempMatrix3;
            for (int i = 0; i < height; i += 8)
            {
                for (int j = 0; j < width; j += 8)
                {
                    tempMatrix = new List<List<double>>();
                    tempMatrix2 = new List<List<double>>();
                    tempMatrix3 = new List<List<double>>();
                   
                    for (int brow = i; brow < i + 8; brow++)
                    {   
                        tempMatrix.Add(new List<double>());
                        tempMatrix2.Add(new List<double>());
                        tempMatrix3.Add(new List<double>());
                         for (int bcol = j; bcol < j + 8; bcol++)
                        {
                            tempMatrix[brow - i].Add(Y[brow][bcol]);
                            tempMatrix2[brow - i].Add(U[brow][bcol]);
                            tempMatrix3[brow - i].Add(V[brow][bcol]);
                          }
                    }
                  
                    yBlocks.Add(new Block(8, 'y', tempMatrix));
                    uBlocks.Add(new Block(8, 'u', tempMatrix2));
                    vBlocks.Add(new Block(8, 'v', tempMatrix3));
                    tempMatrix.Clear();
                    tempMatrix2.Clear();
                    tempMatrix3.Clear();

                }
            }
            List<sbyte> encoding = new List<sbyte>();
            List<int> encodingList = new List<int>();

            for (int i = 0; i < uBlocks.Count; i++)
            {

                Block v = vBlocks[i];
                Block u = uBlocks[i];
                Block y = yBlocks[i];
                u.subsampling(2);
                v.subsampling(2);

                u.oversampling(2);
                v.oversampling(2);

                y.subtractAll(128);
                u.subtractAll(128);
                v.subtractAll(128);

                y.DTCtransformation();
                u.DTCtransformation();
                v.DTCtransformation();


                y.Quantization(Program.quantifier);
                u.Quantization(Program.quantifier);
                v.Quantization(Program.quantifier);
                encodingList.AddRange(y.zigZag());
                encodingList.AddRange(u.zigZag());
                encodingList.AddRange(v.zigZag());
                encoding.AddRange(Block.encodedZigZag(y.zigZag()));
                encoding.AddRange(Block.encodedZigZag(u.zigZag()));
                encoding.AddRange(Block.encodedZigZag(v.zigZag()));
            }

            var decodingList = Block.decodeZigZag(encoding);
         
            var newBlocks = Block.fromDecodingToBlocks(decodingList);
            yBlocks = newBlocks[0];
            uBlocks = newBlocks[1];
            vBlocks = newBlocks[2];
            for (int i = 0; i < uBlocks.Count; i++)
   
                {
                Block v = vBlocks[i];
                Block u = uBlocks[i];
                Block y = yBlocks[i];

                //decode
                y.DEQuantization(Program.quantifier);
                u.DEQuantization (Program.quantifier);
                v.DEQuantization(Program.quantifier);

                y.IDTCtransformation();
                u.IDTCtransformation();
                v.IDTCtransformation();
                

                y.addAll(128);
                u.addAll(128);
                v.addAll(128);
            }

            //decoder

            RGB[][] colorMatrix = new RGB[height][];
            for (int i = 0; i < height; i++) {
                colorMatrix[i] = new RGB[width];
            }
            string[] linesResulted = new string[lines.Length];
            linesResulted[0] = lines[0];
            linesResulted[1] = lines[1];
            linesResulted[2] = lines[2];
            linesResulted[3] = lines[3];
            int roww = 4, rowStart, colStart;
            for (int i = 0; i < uBlocks.Count; i++)
            {

                Block y = yBlocks[i];
                Block u = uBlocks[i];
                Block v = vBlocks[i];
                rowStart = (i * y.Size / width)*y.Size;
                colStart = (i * y.Size) % width;

                for (int k = 0; k < y.Size; k++)
                {

                    for (int z = 0; z < y.Size; z++)
                    {

                        RGB colorr = toRGB(y.Blocks[k][z], u.Blocks[k][z], v.Blocks[k][z]);
                        colorMatrix[k + rowStart][z + colStart] = colorr;
                    }
                }

            }

            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    linesResulted[roww++] = colorMatrix[i][j].Red +"";
                    linesResulted[roww++] = colorMatrix[i][j].Green+"";
                    linesResulted[roww++] = colorMatrix[i][j].Blue+"";
                }
            }

            System.IO.File.WriteAllLines(@"nt-P3-Result.ppm", linesResulted);

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
        }
    }
}
