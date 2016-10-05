﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhotoGallery.Domain;
using PhotoGallery.Persistence.Interfaces;
using PhotoGallery.Services.Interfaces;

namespace PhotoGallery.Services.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PhotoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Photo> GetPhotosOfTheUser(string userId)
        {
            return _unitOfWork.Photos.GetPhotosByUserId(userId).ToList();
        }

        public byte[] GetLargePhotoInBytesById(int photoId)
        {
            return _unitOfWork.Photos.GetLargePhotoById(photoId);
        }

        public Photo GetPhotoById(int id)
        {
            return _unitOfWork.Photos.Get(id);
        }

        public void Modify(Photo photo)
        {
            Photo originalPhoto = _unitOfWork.Photos.Get(photo.PhotoId);
            originalPhoto.Description = photo.Description;
            _unitOfWork.Photos.Modify(originalPhoto);
            _unitOfWork.Complete();
        }

        public void Remove(Photo photo)
        {
            _unitOfWork.Photos.Remove(photo);
            _unitOfWork.Complete();
        }

        public void Dispose()
        {
            _unitOfWork.Dispose();
        }

        public void AddPhoto(Photo model, Stream inputStream, string fileName)
        {
            var newFileName = Guid.NewGuid().ToString();
            var extension = System.IO.Path.GetExtension(fileName).ToLower();

            using (var img = System.Drawing.Image.FromStream(inputStream))
            {
                // Save thumbnail size image, 100 x 100
                // Get new resolution
                Size imgSize = NewImageSize(img.Size, new Size(100, 100));

                using (System.Drawing.Image newImg = new Bitmap(img, imgSize.Width, imgSize.Height))
                {
                    model.ThumbPhoto = ImageToByteArray(newImg);
                }

                // Save large size image, 800 x 800
                // Get new resolution
                Size bigOmgSize = NewImageSize(img.Size, new Size(800, 800));

                using (System.Drawing.Image newImg = new Bitmap(img, imgSize.Width, imgSize.Height))
                {
                    model.LargePhoto = ImageToByteArray(newImg);
                }

                // Save record to database
                model.CreatedOn = DateTime.Now;

                _unitOfWork.Photos.Add(model);
                _unitOfWork.Complete();
            }
        }

        private Size NewImageSize(Size imageSize, Size newSize)
        {
            Size finalSize;
            double tempval;
            if (imageSize.Height > newSize.Height || imageSize.Width > newSize.Width)
            {
                if (imageSize.Height > imageSize.Width)
                    tempval = newSize.Height / (imageSize.Height * 1.0);
                else
                    tempval = newSize.Width / (imageSize.Width * 1.0);

                finalSize = new Size((int)(tempval * imageSize.Width), (int)(tempval * imageSize.Height));
            }
            else
                finalSize = imageSize; // image is already small size

            return finalSize;
        }

        private byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
    }
}