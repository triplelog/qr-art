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
        IImage Create(string data, IRgb24BitmapBase image);
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

        public virtual IImage Create(string data, IRgb24BitmapBase image)
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
