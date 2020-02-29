using Lapis.QRCode.Encoding;
using Lapis.QRCode.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lapis.QRCode.Art
{
    public interface IQRArtCreator
    {
        IImage Create(string data, IRgb24BitmapBase image, IRgb24BitmapBase imageText);
    }

    public class QRArtCreator : IQRArtCreator
    {
        public QRArtCreator(
            IQRCodeEncoder qrCodeEncoder,
            IBinarizer binarizer, ITriparizer triparizer, IColorizer colorizer, IMerger merger,
            IBitMatrixDrawer bitMatrixDrawer, ITripMatrixDrawer tripMatrixDrawer)
        {
            if (qrCodeEncoder == null)
                throw new ArgumentNullException(nameof(qrCodeEncoder));
            if (binarizer == null)
                throw new ArgumentNullException(nameof(binarizer));
            if (colorizer == null)
                throw new ArgumentNullException(nameof(colorizer));
            if (triparizer == null)
                throw new ArgumentNullException(nameof(triparizer));
            if (merger == null)
                throw new ArgumentNullException(nameof(merger));
            if (bitMatrixDrawer == null)
                throw new ArgumentNullException(nameof(bitMatrixDrawer));
            QRCodeEncoder = qrCodeEncoder;
            Binarizer = binarizer;
            Triparizer = triparizer;
            Colorizer = colorizer;
            Merger = merger;
            BitMatrixDrawer = bitMatrixDrawer;
            TripMatrixDrawer = tripMatrixDrawer;
        }

        public IQRCodeEncoder QRCodeEncoder { get; }

        public IBinarizer Binarizer { get; }
        
        public ITriparizer Triparizer { get; }
        
        public IColorizer Colorizer { get; }

        public IMerger Merger { get; }

        public IBitMatrixDrawer BitMatrixDrawer { get; }
        
        public ITripMatrixDrawer TripMatrixDrawer { get; }

        public virtual IImage Create(string data, IRgb24BitmapBase image, IRgb24BitmapBase imageText)
        {
            var bitMatrix = QRCodeEncoder.Build(data);
            if (image != null && 2 == 3)
            {
                int moduleCount = bitMatrix.Size;
                var imgBitMatrix = Binarizer.Binarize(image, moduleCount * 3, moduleCount * 3);
                var imgColorMatrix = Colorizer.Colorize(image, moduleCount * 3, moduleCount * 3);
                //var imgColorMatrix = new ColorSquare(moduleCount * 3);
                bitMatrix = Merger.Merge(bitMatrix, QRCodeEncoder.TypeNumber, imgBitMatrix);
                return BitMatrixDrawer.Draw(bitMatrix, imgColorMatrix);
            }
            else if (image != null)
            {
                int moduleCount = bitMatrix.Size;
                //var imgBitMatrix = Binarizer.Binarize(image, moduleCount * 3, moduleCount * 3);
                var imgColorMatrix = Colorizer.Colorize(image, moduleCount * 3, moduleCount * 3);
                //var imgColorMatrix = new ColorSquare(moduleCount * 3);
                var tripMatrix = new TripSquare(moduleCount * 3);
                for (var i=0;i<moduleCount * 3;i++){
                	for (var ii=0;ii<moduleCount * 3;ii++){
                		tripMatrix[i,ii] = 0;
                	}
                }
                for (var i=10;i<20;i++){
                	for (var ii=10;ii<20;ii++){
                		tripMatrix[i,ii] = imageText.GetPixel(i,ii);
                		Console.WriteLine(tripMatrix[i,ii]);
                	}
                }

                //for (var i=40;i<90;i++){
                //	for (var ii=40;ii<90;ii++){
                //		var d = 50-i
                //		tripMatrix[i,ii] = -1;
                //	}
                //}
                for (var i=50;i<80;i++){
                	for (var ii=50;ii<80;ii++){
                		tripMatrix[i,ii] = 2;
                	}
                }
                for (var i=110;i<130;i++){
                	for (var ii=110;ii<130;ii++){
                		tripMatrix[i,ii] = 2;
                	}
                }
                for (var i=5;i<moduleCount * 3-5;i++){
                	for (var ii=5;ii<moduleCount * 3-5;ii++){
                		if (tripMatrix[i,ii] == 0){
                			var minDist = 999;
                			for (var iii=i-5;iii<i+6;iii++){
								for (var iiii=ii-5;iiii<ii+6;iiii++){
									if (tripMatrix[iii,iiii] > 0){
										var d = (i-iii)*(i-iii)+(ii-iiii)*(ii-iiii);
										if (d< minDist){
											minDist = d;
										}
									}
								}
							}
							if (minDist < 51){
								tripMatrix[i,ii] = (2*minDist-150)/5;
							}
                		}
                	}
                }
                
                return TripMatrixDrawer.Draw(tripMatrix, imgColorMatrix);
            }
            else {
            	Console.Out.WriteLine("no image");
            	var imgMatrix = new ColorSquare(bitMatrix.Size * 3);
            	return BitMatrixDrawer.Draw(bitMatrix, imgMatrix);
            }
        }
    }
}
