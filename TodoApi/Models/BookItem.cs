
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
namespace BooksApi.Models
{
    /*Every book item has a name, rating, genre and category
    the category is a 2d double array, the first row of the array
    contains a rating given to each genre category
    the second row has a 1 for every genre category of the book
    the catgeory is set whenever an update or create is made */
    public class BookItem
    {
        public long Id { get; set; }
        public string Name { get;set;}
        public double rating { get;set;}
        public string genre { get;set;}
        public double trueRating { get;set;}
         [EditorBrowsable(EditorBrowsableState.Never)]
        public string InternalData { get; set; }
        [NotMapped]
        public double[] category{get{try{return Array.ConvertAll(InternalData.Split(';'), Double.Parse);}
        catch{return null;}  }

        set{ 

            double[] _category = new double[BookType.bookCategory.Length];
            for (int i= 0; i < value.Length;i++){
                for (int j=1;j<_category.Length;j++){
                    if ((int)value[i] <= BookType.bookCategory[j] && (int)value[i] > BookType.bookCategory[j-1])
                        _category[j]=rating;
                         
                }     }
                InternalData = String.Join(";", _category.Select(p => p.ToString()).ToArray());
                
        }  }
         [EditorBrowsable(EditorBrowsableState.Never)]
        public string InternalData2 { get; set; }
        [NotMapped]
        public double[] category_def{get{try{return Array.ConvertAll(InternalData2.Split(';'), Double.Parse);}
        catch{return null;} }

        
        set{ double[] _category_def = new double[BookType.bookCategory.Length];
            for (int i= 0; i < value.Length;i++){
                for (int j=1;j<_category_def.Length;j++){
                    if ((int)value[i] <= BookType.bookCategory[j] && (int)value[i] > BookType.bookCategory[j-1])
                        _category_def[j] = 1; /*Can't have the same genre twice, can only equal 1 */ }
                    }
                    InternalData2 = String.Join(";", _category_def.Select(p => p.ToString()).ToArray());
        }  }
        
        

    }
}