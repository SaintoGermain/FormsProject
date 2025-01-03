using FormsProyect.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FormsProyect.Data
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
        
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Forms> Forms { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Topics> Topics { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<FormTags> FormTags { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Likes> Likes { get; set; }
        public DbSet<AllowedUsers> AllowedUsers { get; set; }
        public DbSet<FormFilled> FormFilled { get; set; }
        public DbSet<AnsweredQuestions> AnsweredQuestions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(tb => {
                tb.HasKey(col => new { col.UserId});


                tb.Property(col => col.UserId)
                    .ValueGeneratedOnAdd();
                tb.Property(col => col._Name)
                    .HasMaxLength(50)
                    .IsRequired();
                tb.Property(col => col.Email)
                    .HasMaxLength(50)
                    .IsRequired();
                tb.Property(col => col.PasswordHash)
                    .HasMaxLength(50)
                    .IsRequired();

                tb.HasMany(col => col.Forms)
                    .WithOne(col => col.Users)
                    .HasForeignKey(col => col.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasMany(col => col.AllowedUsers)
                    .WithOne(col => col.Users)
                    .HasForeignKey(col => new { col.UserId, col.NoForm })
                    .OnDelete(DeleteBehavior.Cascade);

                tb.HasMany(col => col.FormFilled)
                    .WithOne(col => col.Users)
                    .HasForeignKey(col => col.NoFilledForm)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Users>().ToTable("Users");


            modelBuilder.Entity<Forms>(tb => {
                tb.HasKey(col => col.NoForm);

                tb.Property(col => col.NoForm)
                    .ValueGeneratedOnAdd();
                tb.Property(col => col.Title)
                    .HasMaxLength(50)
                    .IsRequired();
                tb.Property(col => col.Descr)
                    .HasMaxLength(50)
                    .IsRequired();
                tb.Property(col => col.ImagePath)
                    .HasMaxLength(255);
                tb.Property(col => col.IsPublic);

                tb.HasOne(col => col.Users)
                    .WithMany(col => col.Forms)
                    .HasForeignKey(col => col.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                tb.HasOne(col => col.Topics)
                    .WithMany(col => col.Forms)
                    .HasForeignKey(col => col.TopicID)
                    .OnDelete(DeleteBehavior.Restrict);

                tb.HasMany(col => col.FormTags)
                    .WithOne(col => col.Forms)
                    .HasForeignKey(col => new { col.NoForm, col.TagID });

            });
            modelBuilder.Entity<Forms>().ToTable("Forms");


            modelBuilder.Entity<Questions>(tb => {
                tb.HasKey(col => new { col.IDQuest});


                tb.Property(col => col.IDQuest)
                    .ValueGeneratedOnAdd();
                tb.Property(col => col.TitleQ)
                    .HasMaxLength(50)
                    .IsRequired();
                tb.Property(col => col.DescrQ)
                    .HasMaxLength(50)
                    .IsRequired();
                tb.Property(col => col._Show)
                    .IsRequired();

                tb.HasOne(col => col.Forms)
                    .WithMany(col => col.Questions)
                    .HasForeignKey(col => col.NoForm)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Questions>().ToTable("Questions");

            modelBuilder.Entity<Topics>(tb => {
                tb.HasKey(col => new { col.TopicID });


                tb.Property(col => col.TopicID)
                    .ValueGeneratedOnAdd();
                tb.Property(col => col._TopicName)
                    .IsRequired();

                tb.HasMany(col => col.Forms)
                    .WithOne(col => col.Topics);
            });
            modelBuilder.Entity<Topics>().ToTable("Topics");

            modelBuilder.Entity<Tags>(tb => {
                tb.HasKey(col => new { col.TagID });


                tb.Property(col => col.TagID)
                    .ValueGeneratedOnAdd();
                tb.Property(col => col._TagName)
                    .IsRequired();

                tb.HasMany(col => col.FormTags)
                    .WithOne(col => col.Tags)
                    .HasForeignKey(col => new { col.NoForm, col.TagID });
            });
            modelBuilder.Entity<Tags>().ToTable("Tags");

            modelBuilder.Entity<FormTags>(tb =>
            {
                tb.HasKey(col => new { col.NoForm, col.TagID });

                tb.HasOne(col => col.Forms)
                    .WithMany(col => col.FormTags)
                    .HasForeignKey(col => col.NoForm);

                tb.HasOne(col => col.Tags)
                    .WithMany(col => col.FormTags)
                    .HasForeignKey(col => col.TagID);
            });
            modelBuilder.Entity<FormTags>().ToTable("FormTags");

            modelBuilder.Entity<Comments>(tb =>
            {
                tb.HasKey(col => col.CommentID);

                tb.Property(col => col.Text)
                    .HasMaxLength(30);

                tb.HasOne(col => col.Forms)
                    .WithMany(col => col.Comments)
                    .HasForeignKey(col => col.NoForm);

                tb.HasOne(col => col.Users)
                    .WithMany(col => col.Comments)
                    .HasForeignKey(col => col.UserId);
            });
            modelBuilder.Entity<Comments>().ToTable("Comments");

            modelBuilder.Entity<Likes>(tb =>
            {
                tb.HasKey(col => col.LikeID);

                tb.HasOne(col => col.Forms)
                    .WithMany(col => col.Likes)
                    .HasForeignKey(col => col.NoForm);

                tb.HasOne(col => col.Users)
                    .WithMany(col => col.Likes)
                    .HasForeignKey(col => col.UserId);
            });
            modelBuilder.Entity<Likes>().ToTable("Likes");

            modelBuilder.Entity<AllowedUsers>(tb =>
            {
                tb.HasKey(col => new {col.NoForm, col.UserId});


                tb.HasOne(col => col.Forms)
                    .WithMany(col => col.AllowedUsers)
                    .HasForeignKey(col => col.NoForm);

                tb.HasOne(col => col.Users)
                    .WithMany(col => col.AllowedUsers)
                    .HasForeignKey(col => col.UserId);
            });
            modelBuilder.Entity<AllowedUsers>().ToTable("AllowedUsers");

            modelBuilder.Entity<FormFilled>(tb =>
            {
                tb.HasKey(col => col.NoFilledForm);


                tb.HasOne(col => col.Forms)
                    .WithMany(col => col.FormFilled)
                    .HasForeignKey(col => col.NoForm);

                tb.HasOne(col => col.Users)
                    .WithMany(col => col.FormFilled)
                    .HasForeignKey(col => col.UserId);
            });
            modelBuilder.Entity<FormFilled>().ToTable("FormFilled");

            modelBuilder.Entity<AnsweredQuestions>(tb => {
                tb.HasKey(col => new {col.IDQuest, col.NoFilledForm});

                tb.Property(col => col.QuestT1)
                    .HasMaxLength(15);
                tb.Property(col => col.QuestT2)
                    .HasMaxLength(50);
                tb.Property(col => col.QuestT3);
                tb.Property(col => col.QuestT4);

                tb.HasOne(col => col.Questions)
                    .WithMany(col => col.AnsweredQuestions)
                    .HasForeignKey(col => col.IDQuest)
                    .OnDelete(DeleteBehavior.NoAction);

                tb.HasOne(col => col.FormFilled)
                    .WithMany(col => col.AnsweredQuestions)
                    .HasForeignKey(col => col.NoFilledForm)
                    .OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<AnsweredQuestions>().ToTable("AnsweredQuestions");
        }
    }
}
