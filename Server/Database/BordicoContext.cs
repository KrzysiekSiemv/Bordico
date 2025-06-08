using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Server.Database;

public partial class BordicoContext : DbContext
{
    public BordicoContext()
    {
    }

    public BordicoContext(DbContextOptions<BordicoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Conversation> conversations { get; set; }

    public virtual DbSet<ConversationGroup> conversation_groups { get; set; }

    public virtual DbSet<Friend> friends { get; set; }

    public virtual DbSet<Message> messages { get; set; }

    public virtual DbSet<ReadStatus> read_statuses { get; set; }

    public virtual DbSet<User> users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.id_conversation).HasName("PRIMARY");

            entity.HasIndex(e => e.id_first_user, "id_first_user");

            entity.HasIndex(e => e.id_second_user, "id_second_user");

            entity.Property(e => e.id_conversation).HasColumnType("int(11)");
            entity.Property(e => e.id_first_user).HasColumnType("int(11)");
            entity.Property(e => e.id_second_user).HasColumnType("int(11)");

            entity.HasOne(d => d.id_first_userNavigation).WithMany(p => p.conversationid_first_userNavigations)
                .HasForeignKey(d => d.id_first_user)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("conversations_ibfk_1");

            entity.HasOne(d => d.id_second_userNavigation).WithMany(p => p.conversationid_second_userNavigations)
                .HasForeignKey(d => d.id_second_user)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("conversations_ibfk_2");
        });

        modelBuilder.Entity<ConversationGroup>(entity =>
        {
            entity.HasKey(e => e.id_group).HasName("PRIMARY");

            entity.HasIndex(e => e.id_owner, "id_owner");

            entity.Property(e => e.id_group).HasColumnType("int(11)");
            entity.Property(e => e.group_name)
                .HasMaxLength(60)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.id_owner)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.id_ownerNavigation).WithMany(p => p.conversation_groups)
                .HasForeignKey(d => d.id_owner)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("conversation_groups_ibfk_1");
        });

        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasNoKey();

            entity.HasIndex(e => e.id_first_user, "id_first_user");

            entity.HasIndex(e => e.id_second_user, "id_second_user");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.first_nickname)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text");
            entity.Property(e => e.id_first_user).HasColumnType("int(11)");
            entity.Property(e => e.id_second_user).HasColumnType("int(11)");
            entity.Property(e => e.second_nickname)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text");

            entity.Property(e => e.accepted).HasColumnType("boolean").HasDefaultValueSql("false");

            entity.HasOne(d => d.id_first_userNavigation).WithMany()
                .HasForeignKey(d => d.id_first_user)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("friends_ibfk_1");

            entity.HasOne(d => d.id_second_userNavigation).WithMany()
                .HasForeignKey(d => d.id_second_user)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("friends_ibfk_2");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.id_message).HasName("PRIMARY");

            entity.HasIndex(e => e.id_conversation, "id_conversation");

            entity.HasIndex(e => e.id_group, "id_group");

            entity.HasIndex(e => e.id_sent_by, "id_sent_by");

            entity.Property(e => e.id_message).HasColumnType("int(11)");
            entity.Property(e => e.content)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text");
            entity.Property(e => e.id_conversation)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.id_group)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.id_sent_by)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.sent_at)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");

            entity.HasOne(d => d.id_conversationNavigation).WithMany(p => p.messages)
                .HasForeignKey(d => d.id_conversation)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("messages_ibfk_1");

            entity.HasOne(d => d.id_groupNavigation).WithMany(p => p.messages)
                .HasForeignKey(d => d.id_group)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("messages_ibfk_2");

            entity.HasOne(d => d.id_sent_byNavigation).WithMany(p => p.messages)
                .HasForeignKey(d => d.id_sent_by)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("messages_ibfk_3");
        });

        modelBuilder.Entity<ReadStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("read_status");

            entity.HasIndex(e => e.id_message, "id_message");

            entity.HasIndex(e => e.id_user, "id_user");

            entity.Property(e => e.id_message)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");
            entity.Property(e => e.id_user)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("int(11)");

            entity.HasOne(d => d.id_messageNavigation).WithMany()
                .HasForeignKey(d => d.id_message)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("read_status_ibfk_1");

            entity.HasOne(d => d.id_userNavigation).WithMany()
                .HasForeignKey(d => d.id_user)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("read_status_ibfk_2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.id_user).HasName("PRIMARY");

            entity.Property(e => e.id_user).HasColumnType("int(11)");
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.description)
                .HasDefaultValueSql("'NULL'")
                .HasColumnType("text");
            entity.Property(e => e.email_address).HasMaxLength(128);
            entity.Property(e => e.nickname)
                .HasMaxLength(80)
                .HasDefaultValueSql("'NULL'");
            entity.Property(e => e.password).HasColumnType("text");
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("'current_timestamp()'")
                .HasColumnType("datetime");
            entity.Property(e => e.username).HasMaxLength(24);
            entity.Property(e => e.allow_messages).HasColumnType("boolean").HasDefaultValueSql("true");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
