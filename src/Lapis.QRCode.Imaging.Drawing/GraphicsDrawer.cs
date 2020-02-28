using Lapis.QRCode.Encoding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lapis.QRCode.Imaging.Drawing
{
    public class GraphicsDrawer : BitMatrixDrawerBase
    {
        public override IImage Draw(BitMatrix bitMatrix, ColorMatrix colorMatrix)
        {
            if (bitMatrix == null)
                throw new ArgumentNullException(nameof(bitMatrix));
			
            int rowCount = bitMatrix.RowCount;
            int columnCount = bitMatrix.ColumnCount;
            int imageHeight = CellSize * rowCount + Margin * 2;
            int imageWidth = CellSize * rowCount + Margin * 2;
            var bitmap = new Bitmap(imageHeight, imageWidth);
			Console.WriteLine(CellSize);
			Console.WriteLine(rowCount);
			Console.WriteLine(imageHeight);
            using (var graph = Graphics.FromImage(bitmap))
            {
                graph.Clear(ColorHelper.FromIntRgb24(Background));
                //var foreBrush = new SolidBrush(ColorHelper.FromIntRgb24(Foreground));
                var foreBrush = new SolidBrush(Color.FromArgb(40,40,40));
                var foreBrushB = new SolidBrush(Color.FromArgb(0,0,120));
                for (var r = 0; r < rowCount; r += 1)
                {
                    for (var c = 0; c < columnCount; c += 1)
                    {
                        if (bitMatrix[r, c])
                        {
                            var x = Margin + c * CellSize;
                            var y = Margin + r * CellSize;
                            for (var cmi = 0;cmi<CellSize;cmi++){
                            	for (var cmj = 0;cmj<CellSize;cmj++){
									int re = (colorMatrix[CellSize*r+cmi,CellSize*c+cmj] & 0xFF0000) >> 16;
									int gr = (colorMatrix[CellSize*r+cmi,CellSize*c+cmj] & 0xFF00) >> 8;
									int bl = colorMatrix[CellSize*r+cmi,CellSize*c+cmj] & 0xFF;
							
									//Darken uniformly
									re = re/5;
									gr = gr/5;
									bl = bl/5;
							
									var foreBrushCustom = new SolidBrush(Color.FromArgb(re,gr,bl));
									graph.FillRectangle(foreBrushCustom, x+cmi, y+cmj, 1,1);
								}
							}
                        }
                        else {
                        	var x = Margin + c * CellSize;
                            var y = Margin + r * CellSize;
                            for (var cmi = 0;cmi<CellSize;cmi++){
                            	for (var cmj = 0;cmj<CellSize;cmj++){
									int re = (colorMatrix[CellSize*r+cmi,CellSize*c+cmj] & 0xFF0000) >> 16;
									int gr = (colorMatrix[CellSize*r+cmi,CellSize*c+cmj] & 0xFF00) >> 8;
									int bl = colorMatrix[CellSize*r+cmi,CellSize*c+cmj] & 0xFF;
							
									//Darken uniformly
									re = 255 - (255-re)/5;
									gr = 255 - (255-gr)/5;
									bl = 255 - (255-bl)/5;
							
									var foreBrushCustom = new SolidBrush(Color.FromArgb(re,gr,bl));
									graph.FillRectangle(foreBrushCustom, x+cmi, y+cmj, 1,1);
								}
							}
                        }
                    }
                }
            }

            return new BitmapFrame(bitmap);
        }         
        
    }
}
