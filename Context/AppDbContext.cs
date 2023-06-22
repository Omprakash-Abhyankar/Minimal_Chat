using Minimal_Chat_App.Models;
using Microsoft.EntityFrameworkCore;
using System;


namespace Minimal_Chat_App.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


                    

          
            // Configure the User entity
            modelBuilder.Entity<Users>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<Users>()
                .Property(u => u.UserId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Users>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<Users>()
                .HasIndex(u => u.Email)
                .IsUnique();

            //// Configure the Message entity
            //modelBuilder.Entity<Message>()
            //    .HasKey(m => m.MessageId);

            //modelBuilder.Entity<Message>()
            //    .Property(m => m.MessageId)
            //    .ValueGeneratedOnAdd();

            //modelBuilder.Entity<Message>()
            //    .Property(m => m.SenderId)
            //    .IsRequired();

            //modelBuilder.Entity<Message>()
            //    .Property(m => m.ReceiverId)
            //    .IsRequired();

            //modelBuilder.Entity<Message>()
            //    .Property(m => m.Content)
            //    .IsRequired();

            //modelBuilder.Entity<Message>()
            //    .Property(m => m.Timestamp)
            //    .IsRequired();

            base.OnModelCreating(modelBuilder);


            //modelBuilder.Entity<Message>()
            //    .HasOne(m => m.Sender)
            //    .WithMany(u => u.SentMessages)
            //    .HasForeignKey(m => m.SenderId)
            //    .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Message>()
            //    .HasOne(m => m.Receiver)
            //    .WithMany(u => u.ReceivedMessages)
            //    .HasForeignKey(m => m.ReceiverId)
            //    .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<SendMessageRequest>()
                    .HasNoKey();

            modelBuilder.Entity<SendMessageResponse>()
                   .HasNoKey();


        }




        public DbSet<Users> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Login> Logins { get; set; }
         
        public DbSet<Minimal_Chat_App.Models.Message> Chat { get; set; } = default!;
        //public DbSet<Conversation> Conversations { get; set; }
        //public DbSet<SQuery> SQuery { get; set; }   
        //public DbSet<ReseachBook> ReseachBook { get; set;}
        //public DbSet<LegalInfo> LegalInfo { get; set; } 
        //public DbSet<Chat> Chat { get; set; }   


    }


   
}
