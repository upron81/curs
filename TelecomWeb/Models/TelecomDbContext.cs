using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace TelecomWeb.Models;

public partial class TelecomDbContext : IdentityDbContext<IdentityUser>
//public partial class TelecomDbContext : DbContext
{
    public TelecomDbContext()
    {
    }

    public TelecomDbContext(DbContextOptions<TelecomDbContext> options)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.CallId).HasName("PK__Calls__5180CF8A3D2D2ED2");

            entity.Property(e => e.CallId).HasColumnName("CallID");
            entity.Property(e => e.CallDate).HasColumnType("datetime");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");

            entity.HasOne(d => d.Contract).WithMany(p => p.Calls)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__Calls__ContractI__14B10FFA");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK__Contract__C90D34095C346A02");

            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.SubscriberId).HasColumnName("SubscriberID");
            entity.Property(e => e.TariffPlanId).HasColumnName("TariffPlanID");

            entity.HasOne(d => d.Staff).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.StaffId)
                .HasConstraintName("FK__Contracts__Staff__11D4A34F");

            entity.HasOne(d => d.Subscriber).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.SubscriberId)
                .HasConstraintName("FK__Contracts__Subsc__0FEC5ADD");

            entity.HasOne(d => d.TariffPlan).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.TariffPlanId)
                .HasConstraintName("FK__Contracts__Tarif__10E07F16");
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
            entity.HasKey(e => e.UsageId).HasName("PK__Internet__29B197C0F4400AED");

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
                .HasConstraintName("FK__InternetU__Contr__1A69E950");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C037C0167DB96");

            entity.Property(e => e.MessageId).HasColumnName("MessageID");
            entity.Property(e => e.ContractId).HasColumnName("ContractID");
            entity.Property(e => e.IsMms).HasColumnName("IsMMS");
            entity.Property(e => e.MessageDate).HasColumnType("datetime");

            entity.HasOne(d => d.Contract).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK__Messages__Contra__178D7CA5");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__96D4AAF7835BD125");

            entity.Property(e => e.StaffId).HasColumnName("StaffID");
            entity.Property(e => e.Education).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.PositionId).HasColumnName("PositionID");

            entity.HasOne(d => d.Position).WithMany(p => p.Staff)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("FK__Staff__PositionI__093F5D4E");
        });

        modelBuilder.Entity<StaffPosition>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__StaffPos__60BB9A59D4D3AD5C");

            entity.ToTable("StaffPosition");

            entity.Property(e => e.PositionId).HasColumnName("PositionID");
            entity.Property(e => e.PositionName).HasMaxLength(100);
        });

        modelBuilder.Entity<Subscriber>(entity =>
        {
            entity.HasKey(e => e.SubscriberId).HasName("PK__Subscrib__7DFEB634B3799F70");

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
            entity.HasKey(e => e.TariffPlanId).HasName("PK__TariffPl__29A9282A1C614268");

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
