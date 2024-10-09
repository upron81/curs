using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TelecomApp.Models;

public partial class Db8328Context : DbContext
{
    public Db8328Context()
    {
    }

    public Db8328Context(DbContextOptions<Db8328Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Call> Calls { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<ContractCall> ContractCalls { get; set; }

    public virtual DbSet<ContractInternetUsage> ContractInternetUsages { get; set; }

    public virtual DbSet<ContractMessage> ContractMessages { get; set; }

    public virtual DbSet<EmployeeInfo> EmployeeInfos { get; set; }

    public virtual DbSet<InternetUsage> InternetUsages { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    public virtual DbSet<StaffPosition> StaffPositions { get; set; }

    public virtual DbSet<Subscriber> Subscribers { get; set; }

    public virtual DbSet<SubscriberInfo> SubscriberInfos { get; set; }

    public virtual DbSet<TariffPlan> TariffPlans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Загрузка конфигурации из файла appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Получение строки подключения
        var connectionString = configuration.GetConnectionString("TelecomDatabase");
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.CallId).HasName("PK__Calls__5180CF8A4C1A2D2C");

            entity.Property(e => e.CallId).HasColumnName("CallID");
            entity.Property(e => e.CallDate).HasColumnType("datetime");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");

            entity.HasOne(d => d.Contract).WithMany(p => p.Calls)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Calls__ContractI__3A4CA8FD");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D34098518ECB7");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.SubscriberId).HasColumnName("SubscriberID");
            entity.Property(e => e.TariffPlanId).HasColumnName("TariffPlanID");

            entity.HasOne(d => d.Staff).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Staff__37703C52");

            entity.HasOne(d => d.Subscriber).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.SubscriberId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Contracts__Subsc__3587F3E0");

            entity.HasOne(d => d.TariffPlan).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.TariffPlanId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Contracts__Tarif__367C1819");
        });

        modelBuilder.Entity<ContractCall>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ContractCalls");

            entity.Property(e => e.CallDate).HasColumnType("datetime");
            entity.Property(e => e.CallId).HasColumnName("CallID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<ContractInternetUsage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ContractInternetUsage");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.DataReceivedMb)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DataReceivedMB");
            entity.Property(e => e.DataSentMb)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DataSentMB");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.UsageDate).HasColumnType("datetime");
            entity.Property(e => e.UsageId).HasColumnName("UsageID");
        });

        modelBuilder.Entity<ContractMessage>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ContractMessages");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.IsMms).HasColumnName("IsMMS");
            entity.Property(e => e.MessageDate).HasColumnType("datetime");
            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
        });

        modelBuilder.Entity<EmployeeInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("EmployeeInfo");

            entity.Property(e => e.Education).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.PositionName).HasMaxLength(100);
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
        });

        modelBuilder.Entity<InternetUsage>(entity =>
        {
            entity.HasKey(e => e.UsageId).HasName("PK__Internet__29B197C071A2003F");

            entity.ToTable("InternetUsage");

            entity.Property(e => e.UsageId).HasColumnName("UsageID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.DataReceivedMb)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DataReceivedMB");
            entity.Property(e => e.DataSentMb)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DataSentMB");
            entity.Property(e => e.UsageDate).HasColumnType("datetime");

            entity.HasOne(d => d.Contract).WithMany(p => p.InternetUsages)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__InternetU__Contr__40058253");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037CEF80E5DC");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.IsMms).HasColumnName("IsMMS");
            entity.Property(e => e.MessageDate).HasColumnType("datetime");

            entity.HasOne(d => d.Contract).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ContractId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Contra__3D2915A8");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__96D4AAF7C1830A57");

            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.Education).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Position).WithMany(p => p.Staff)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Staff__PositionI__2EDAF651");
        });

        modelBuilder.Entity<StaffPosition>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__StaffPos__60BB9A59C4E44263");

            entity.ToTable("StaffPosition");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PositionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.SubscriberId).HasName("PK__Subscrib__7DFEB634E4D583A6");

            entity.Property(e => e.SubscriberId).HasColumnName("SubscriberID");
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.HomeAddress).HasMaxLength(255);
            entity.Property(e => e.PassportData).HasMaxLength(100);
        });

        modelBuilder.Entity<SubscriberInfo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("SubscriberInfo");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.HomeAddress).HasMaxLength(255);
            entity.Property(e => e.PassportData).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.SubscriberFullName).HasMaxLength(150);
            entity.Property(e => e.SubscriberId).HasColumnName("SubscriberID");
            entity.Property(e => e.TariffName).HasMaxLength(100);
        });

        modelBuilder.Entity<TariffPlan>(entity =>
        {
            entity.HasKey(e => e.TariffPlanId).HasName("PK__TariffPl__29A9282AA46A8AB6");

            entity.Property(e => e.TariffPlanId).HasColumnName("TariffPlanID");
            entity.Property(e => e.DataRatePerMb)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DataRatePerMB");
            entity.Property(e => e.InternationalCallRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LocalCallRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LongDistanceCallRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MmsRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SmsRate).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SubscriptionFee).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TariffName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
