using CommandLine;
using System;
using System.Drawing;
using System.IO;

namespace TileImageMarker
{
    class Program
    {
        public class Options
        {
            [Option('r', "row", Required = false, HelpText = "row count")]
            public int Row { get; set; }

            [Option('c', "col", Required = false, HelpText = "col count")]
            public int Col { get; set; }

            [Option('w', "cell-width", Required = false, HelpText = "cell width")]
            public int CellWidth { get; set; }

            [Option('h', "cell-height", Required = false, HelpText = "cell height")]
            public int CellHeight { get; set; }

            [Option('s', "cell-size", Required = false, HelpText = "cell size")]
            public int CellSize { get; set; }

            [Option('i', "input", Required = true, HelpText = "input image path")]
            public string InputPath { get; set; }

            [Option('o', "output", Required = true, HelpText = "output image path")]
            public string OutputPath { get; set; }
        }

        static void Main(string[] args)
        {
            Options opt = null;
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o => opt = o);

            if (opt == null || opt.InputPath == null || opt.OutputPath == null) {
                Console.WriteLine("use --help to see options.");
                return;
            }

            try {
                var image = Image.FromFile(opt.InputPath);
                int cell_w, cell_h, rows, cols;

                if (opt.CellSize != 0) {
                    cell_w = cell_h = opt.CellSize;
                    rows = image.Height / cell_h;
                    cols = image.Width / cell_w;
                }
                else if (opt.CellWidth != 0 && opt.CellHeight != 0) {
                    cell_w = opt.CellWidth;
                    cell_h = opt.CellHeight;
                    rows = image.Height / cell_h;
                    cols = image.Width / cell_w;
                }
                else if (opt.Row != 0 && opt.Col != 0) {
                    rows = opt.Row;
                    cols = opt.Col;
                    cell_h = image.Height / rows;
                    cell_w = image.Width / cols;
                }
                else {
                    Console.WriteLine("must set row col or cell size.");
                    return;
                }

                using (var graphics = Graphics.FromImage(image)) {
                    var line_pen = new Pen(Color.White, 1f);
                    line_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                    for (var i = 1; i < rows; ++i) {
                        graphics.DrawLine(line_pen, new PointF(0f, i * cell_h), new PointF(image.Width, i * cell_h));
                    }

                    for (var i = 1; i < cols; ++i) {
                        graphics.DrawLine(line_pen, new PointF(i * cell_w, 0f), new PointF(i * cell_w, image.Height));
                    }

                    var text_opt = new StringFormat();
                    text_opt.Alignment = StringAlignment.Center;
                    text_opt.LineAlignment = StringAlignment.Center;

                    var font = new Font("微软雅黑", 10, FontStyle.Regular);

                    for (var i = 0; i < rows; ++i) {
                        for (var j = 0; j < cols; ++j) {
                            var idx = i * cols + j;
                            var rect = new RectangleF(
                                j * cell_w,
                                i * cell_h,
                                cell_w, cell_h);

                            graphics.DrawString(idx.ToString(), font, Brushes.White, rect, text_opt);
                        }
                    }
                }

                image.Save(opt.OutputPath);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}
