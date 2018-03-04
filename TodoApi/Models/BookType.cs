using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
namespace BooksApi.Models

{
    public static class BookType
    {
        /*bookGenres is a list of all the possible book genres
        for any given book, several sub-genres may exist but
        for the purpose of this assignment only these will be used */
        public static string[] bookGenres = {"fantasy","science-fiction", "series", "trilogy", "adventure","comedy","children's",
        "comics", "drama",  "romance", "mystery","crime","horror",
        "self-help","health","guide","travel","religion","prayer books", "spirituality","science","history",
        "math","anthology","poetry","encyclopedias","dictionaries","art","cookbooks",
        "diaries","journals","biographies","autobiographies"} ;

        /*bookCategories is an indexing guide for every genre.
        i have arranged all of the genres into categories
        for example at index 1 (bookCategroy[1]) => includes fantasy
        and science-fiction, bookCategory[2] is series and trilogy.
        working with genre categories helps to identify what type of books 
        someone may like. Some who enjoy fantasy may also enjoy science-fiction*/
        public static int[] bookCategory={-1,1,3,4,5,6,7,9,11,12,14,16,19,20,21,22,23,25,27,29,31};
        /*bookNum_factor is used to decide how many books can be considered relevant to
        infer that someone can like or dislike a particular genre of a book,
        allowing the program to either give a higher rating to books of a certain genre of vis versa*/
        public static double[] bookNum_factor = {0,0.3,0.48,0.65,0.8,1,1.035,1.055};

        /*valid rating between 1 and 10 must be given for each book or else rating is set to 0 */
        public static double approve_Rating(double rating) { 
                /*Error : No valid rating*/
                if (rating < 0 || rating > 10)
                    rating = 0;
                    
                else 
                    rating = Math.Round(rating,2);
            return rating;
            }
        /*valid genres must be given eliminating all genres that does appear in bookGenres
        also initializes category(see Bookitem class) */
        
        public static string approve_Genre(BookItem book) { 
            string newGenre = null;
            book.genre = book.genre.Replace(" ","");
            List<double> category_List = new List<double>();
            string[] genreList = book.genre.Split(",");
            foreach (string bookGenre in genreList){
                for (int i = 0 ;i < BookType.bookGenres.Length;i++) {
                    if (BookType.bookGenres[i] == bookGenre.ToLower()){
                        string comma = null;
                        if (newGenre != null)
                            comma = ",";
                        newGenre = newGenre + comma + BookType.bookGenres[i];
                        category_List.Add(i);
                        break;} 
            } }
            
            double[] cat_list = category_List.ToArray();
            book.category=cat_list;
            book.category_def=cat_list;

            return newGenre;
        }
        /*Generates a list containing plus/minus bonus for every given genre */
        public static void genre_Weights(BooksContext BookList){
            /*Uses all books with a valid rating and creates a table of 
            size 21 containing the avg rating of each genre, the number of books
            per genre and the final weight added to each book genre category */
            var books = BookList.BookItems.ToList();
            double[] rating_List= new double[BookType.bookCategory.Length];
            double[] booknum_per_genre = new double[BookType.bookCategory.Length];
            double[] rating_Bonus = new double [BookType.bookCategory.Length];
            try{
            books.ForEach(delegate(BookItem book)
            {
                if (book.rating > 0 && book.category!= null){
                    booknum_per_genre= booknum_per_genre.Zip(book.category_def, (x,y) => x+y).ToArray();
                    rating_List= rating_List.Zip(book.category, (x,y) => x+y).ToArray();
                    
                    }
            });}
            catch{/*no items in list*/}

            rating_Bonus = rating_List.Zip(booknum_per_genre, (x,y) => x/y).ToArray();
            for (int i=0; i < rating_Bonus.Length;i++){
                if (booknum_per_genre[i]>0 && booknum_per_genre[i]<9)
                    rating_Bonus[i]=((rating_Bonus[i]-5)/4) * bookNum_factor[(int)booknum_per_genre[i]-1];
                else if (booknum_per_genre[i]>8)
                    rating_Bonus[i]=((rating_Bonus[i]-5)/4) * bookNum_factor[(int)booknum_per_genre.Last()];
            }
            BookList.genreRatings = rating_Bonus;

            /*fiction and non-fiction book genres have been divided
            at index 10 in bookCategories so program can extend to 
            adding weights for fiction and non fiction too*/
    }
        /*The rating of every book can be changed according to
        whether or not someone enjoys reading a particular
        kind of book */
        public static void recalculate_Rating(BooksContext BookList){
            
            var books = BookList.BookItems.ToList();
            try{
            books.ForEach(delegate(BookItem book)
            {
                double count = 0;
                double rating =0;
                if (book.genre != null && book.rating>0 && book.rating<=10){
                for (int i =0; i<BookType.bookCategory.Length;i++){
                    if (book.category_def[i]==1 && BookList.genreRatings[i]>0){
                        rating = rating + BookList.genreRatings[i];
                        count = count+1;

                    }
                }   }
                rating = Math.Round(book.rating + (rating/count),2); 
                if(rating >10) rating = 10;
                if (rating < 0) rating = 0;
                book.trueRating = rating;
                
            });}
            catch{/*No items in list*/}
        }

    }

}
