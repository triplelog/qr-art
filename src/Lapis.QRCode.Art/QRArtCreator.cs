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
                var tripMatrix = new TripSquare(500);
                for (var i=0;i<500;i++){
                	for (var ii=0;ii<500;ii++){
                		tripMatrix[i,ii] = 0;
                	}
                }
                
                for (var i=5;i<495;i++){
                	for (var ii=5;ii<495;ii++){
                		if (imageText.GetPixel(i,ii) < 4000000){
                			tripMatrix[ii,i] = 1;
                		}
                	}
                }

                for (var i=5;i<500-5;i++){
                	for (var ii=5;ii<500-5;ii++){
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
