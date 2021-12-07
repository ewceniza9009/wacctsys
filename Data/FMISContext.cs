using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using webfmis.Models;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace webfmis.Data
{
    public partial class FMISContext : DbContext
    {
        public FMISContext()
        {
        }

        public FMISContext(DbContextOptions<FMISContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<MstAccount> MstAccount { get; set; }
        public virtual DbSet<MstAccountArticleType> MstAccountArticleType { get; set; }
        public virtual DbSet<MstAccountCashFlow> MstAccountCashFlow { get; set; }
        public virtual DbSet<MstAccountCategory> MstAccountCategory { get; set; }
        public virtual DbSet<MstAccountType> MstAccountType { get; set; }
        public virtual DbSet<MstArticle> MstArticle { get; set; }
        public virtual DbSet<MstArticleComponent> MstArticleComponent { get; set; }
        public virtual DbSet<MstArticleGroup> MstArticleGroup { get; set; }
        public virtual DbSet<MstArticleInventory> MstArticleInventory { get; set; }
        public virtual DbSet<MstArticleType> MstArticleType { get; set; }
        public virtual DbSet<MstArticleUnit> MstArticleUnit { get; set; }
        public virtual DbSet<MstBranch> MstBranch { get; set; }
        public virtual DbSet<MstCompany> MstCompany { get; set; }
        public virtual DbSet<MstDiscount> MstDiscount { get; set; }
        public virtual DbSet<MstPayType> MstPayType { get; set; }
        public virtual DbSet<MstTaxType> MstTaxType { get; set; }
        public virtual DbSet<MstTerm> MstTerm { get; set; }
        public virtual DbSet<MstUnit> MstUnit { get; set; }
        public virtual DbSet<MstUser> MstUser { get; set; }
        public virtual DbSet<MstUserForm> MstUserForm { get; set; }
        public virtual DbSet<SysAuditTrail> SysAuditTrail { get; set; }
        public virtual DbSet<SysForm> SysForm { get; set; }
        public virtual DbSet<TrnCollection> TrnCollection { get; set; }
        public virtual DbSet<TrnCollectionLine> TrnCollectionLine { get; set; }
        public virtual DbSet<TrnDisbursement> TrnDisbursement { get; set; }
        public virtual DbSet<TrnDisbursementLine> TrnDisbursementLine { get; set; }
        public virtual DbSet<TrnInventory> TrnInventory { get; set; }
        public virtual DbSet<TrnJournal> TrnJournal { get; set; }
        public virtual DbSet<TrnJournalVoucher> TrnJournalVoucher { get; set; }
        public virtual DbSet<TrnJournalVoucherLine> TrnJournalVoucherLine { get; set; }
        public virtual DbSet<TrnPurchaseOrder> TrnPurchaseOrder { get; set; }
        public virtual DbSet<TrnPurchaseOrderItem> TrnPurchaseOrderItem { get; set; }
        public virtual DbSet<TrnReceivingReceipt> TrnReceivingReceipt { get; set; }
        public virtual DbSet<TrnReceivingReceiptItem> TrnReceivingReceiptItem { get; set; }
        public virtual DbSet<TrnSalesInvoice> TrnSalesInvoice { get; set; }
        public virtual DbSet<TrnSalesInvoiceItem> TrnSalesInvoiceItem { get; set; }
        public virtual DbSet<TrnStockCount> TrnStockCount { get; set; }
        public virtual DbSet<TrnStockCountItem> TrnStockCountItem { get; set; }
        public virtual DbSet<TrnStockIn> TrnStockIn { get; set; }
        public virtual DbSet<TrnStockInItem> TrnStockInItem { get; set; }
        public virtual DbSet<TrnStockOut> TrnStockOut { get; set; }
        public virtual DbSet<TrnStockOutItem> TrnStockOutItem { get; set; }
        public virtual DbSet<TrnStockTransfer> TrnStockTransfer { get; set; }
        public virtual DbSet<TrnStockTransferItem> TrnStockTransferItem { get; set; }
        public virtual DbSet<WebMenu> WebMenu { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(Config.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");
            });

            modelBuilder.Entity<MstAccount>(entity =>
            {
                entity.HasOne(d => d.AccountCashFlow)
                    .WithMany(p => p.MstAccount)
                    .HasForeignKey(d => d.AccountCashFlowId)
                    .HasConstraintName("FK_MstAccount_MstAccountCashFlow");

                entity.HasOne(d => d.AccountType)
                    .WithMany(p => p.MstAccount)
                    .HasForeignKey(d => d.AccountTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstAccount_MstAccountType");
            });

            modelBuilder.Entity<MstAccountArticleType>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.MstAccountArticleType)
                    .HasForeignKey(d => d.AccountId)
                    .HasConstraintName("FK_MstAccountArticleType_MstAccount");

                entity.HasOne(d => d.ArticleType)
                    .WithMany(p => p.MstAccountArticleType)
                    .HasForeignKey(d => d.ArticleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstAccountArticleType_MstArticleType");
            });

            modelBuilder.Entity<MstAccountType>(entity =>
            {
                entity.HasOne(d => d.AccountCategory)
                    .WithMany(p => p.MstAccountType)
                    .HasForeignKey(d => d.AccountCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstAccountType_MstAccountCategory");
            });

            modelBuilder.Entity<MstArticle>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.MstArticleAccount)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstAccount");

                entity.HasOne(d => d.ArticleGroup)
                    .WithMany(p => p.MstArticle)
                    .HasForeignKey(d => d.ArticleGroupId)
                    .HasConstraintName("FK_MstArticle_MstArticleGroup");

                entity.HasOne(d => d.ArticleType)
                    .WithMany(p => p.MstArticle)
                    .HasForeignKey(d => d.ArticleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstArticleType");

                entity.HasOne(d => d.AssetAccount)
                    .WithMany(p => p.MstArticleAssetAccount)
                    .HasForeignKey(d => d.AssetAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstAccount3");

                entity.HasOne(d => d.CostAccount)
                    .WithMany(p => p.MstArticleCostAccount)
                    .HasForeignKey(d => d.CostAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstAccount2");

                entity.HasOne(d => d.InputTax)
                    .WithMany(p => p.MstArticleInputTax)
                    .HasForeignKey(d => d.InputTaxId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstTaxType1");

                entity.HasOne(d => d.OutputTax)
                    .WithMany(p => p.MstArticleOutputTax)
                    .HasForeignKey(d => d.OutputTaxId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstTaxType");

                entity.HasOne(d => d.SalesAccount)
                    .WithMany(p => p.MstArticleSalesAccount)
                    .HasForeignKey(d => d.SalesAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstAccount1");

                entity.HasOne(d => d.Term)
                    .WithMany(p => p.MstArticle)
                    .HasForeignKey(d => d.TermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstTerm");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.MstArticle)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstUnit");

                entity.HasOne(d => d.WtaxType)
                    .WithMany(p => p.MstArticleWtaxType)
                    .HasForeignKey(d => d.WtaxTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticle_MstTaxType2");
            });

            modelBuilder.Entity<MstArticleComponent>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.MstArticleComponentArticle)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_MstArticleComponent_MstArticle");

                entity.HasOne(d => d.ComponentArticle)
                    .WithMany(p => p.MstArticleComponentComponentArticle)
                    .HasForeignKey(d => d.ComponentArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticleComponent_MstArticle1");
            });

            modelBuilder.Entity<MstArticleGroup>(entity =>
            {
                entity.HasOne(d => d.ArticleType)
                    .WithMany(p => p.MstArticleGroup)
                    .HasForeignKey(d => d.ArticleTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticleGroup_MstArticleType");
            });

            modelBuilder.Entity<MstArticleInventory>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.MstArticleInventory)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_MstArticleInventory_MstArticle");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.MstArticleInventory)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticleInventory_MstBranch");
            });

            modelBuilder.Entity<MstArticleUnit>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.MstArticleUnit)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_MstArticleUnit_MstArticle");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.MstArticleUnit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstArticleUnit_MstUnit");
            });

            modelBuilder.Entity<MstBranch>(entity =>
            {
                entity.HasOne(d => d.Company)
                    .WithMany(p => p.MstBranch)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_MstBranch_MstCompany");
            });

            modelBuilder.Entity<MstDiscount>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.MstDiscount)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstDiscount_MstAccount");
            });

            modelBuilder.Entity<MstPayType>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.MstPayType)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstPayType_MstAccount");
            });

            modelBuilder.Entity<MstTaxType>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.MstTaxType)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstTaxType_MstAccount");
            });

            modelBuilder.Entity<MstUserForm>(entity =>
            {
                entity.HasOne(d => d.Form)
                    .WithMany(p => p.MstUserForm)
                    .HasForeignKey(d => d.FormId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MstUserForm_SysForm");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.MstUserForm)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_MstUserForm_MstUser");
            });

            modelBuilder.Entity<SysAuditTrail>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.SysAuditTrail)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SysAuditTrail_SysAuditTrail");
            });

            modelBuilder.Entity<TrnCollection>(entity =>
            {
                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnCollection)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnCollection_MstBranch");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TrnCollection)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnCollection_MstArticle");
            });

            modelBuilder.Entity<TrnCollectionLine>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TrnCollectionLine)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnCollectionLine_MstAccount");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TrnCollectionLineArticle)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnCollectionLine_MstArticle");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnCollectionLine)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnCollectionLine_MstBranch");

                entity.HasOne(d => d.DepositoryBank)
                    .WithMany(p => p.TrnCollectionLineDepositoryBank)
                    .HasForeignKey(d => d.DepositoryBankId)
                    .HasConstraintName("FK_TrnCollectionLine_MstArticle1");

                entity.HasOne(d => d.Or)
                    .WithMany(p => p.TrnCollectionLine)
                    .HasForeignKey(d => d.Orid)
                    .HasConstraintName("FK_TrnCollectionLine_TrnCollection");

                entity.HasOne(d => d.PayType)
                    .WithMany(p => p.TrnCollectionLine)
                    .HasForeignKey(d => d.PayTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnCollectionLine_MstPayType");

                entity.HasOne(d => d.Si)
                    .WithMany(p => p.TrnCollectionLine)
                    .HasForeignKey(d => d.Siid)
                    .HasConstraintName("FK_TrnCollectionLine_TrnSalesInvoice");
            });

            modelBuilder.Entity<TrnDisbursement>(entity =>
            {
                entity.HasOne(d => d.Bank)
                    .WithMany(p => p.TrnDisbursementBank)
                    .HasForeignKey(d => d.BankId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnDisbursement_MstArticle1");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnDisbursement)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnDisbursement_MstBranch");

                entity.HasOne(d => d.PayType)
                    .WithMany(p => p.TrnDisbursement)
                    .HasForeignKey(d => d.PayTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnDisbursement_MstPayType");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.TrnDisbursementSupplier)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnDisbursement_MstArticle");
            });

            modelBuilder.Entity<TrnDisbursementLine>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TrnDisbursementLine)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnDisbursementLine_MstAccount");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TrnDisbursementLine)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnDisbursementLine_MstArticle");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnDisbursementLine)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnDisbursementLine_MstBranch");

                entity.HasOne(d => d.Cv)
                    .WithMany(p => p.TrnDisbursementLine)
                    .HasForeignKey(d => d.Cvid)
                    .HasConstraintName("FK_TrnDisbursementLine_TrnDisbursement");

                entity.HasOne(d => d.Rr)
                    .WithMany(p => p.TrnDisbursementLine)
                    .HasForeignKey(d => d.Rrid)
                    .HasConstraintName("FK_TrnDisbursementLine_TrnReceivingReceipt");
            });

            modelBuilder.Entity<TrnInventory>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnInventory_MstArticle");

                entity.HasOne(d => d.ArticleInventory)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.ArticleInventoryId)
                    .HasConstraintName("FK_TrnInventory_MstArticleInventory");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnInventory_MstBranch");

                entity.HasOne(d => d.In)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.Inid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnInventory_TrnStockIn");

                entity.HasOne(d => d.Ot)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.Otid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnInventory_TrnStockOut");

                entity.HasOne(d => d.Rr)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.Rrid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnInventory_TrnReceivingReceipt");

                entity.HasOne(d => d.Si)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.Siid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnInventory_TrnSalesInvoice");

                entity.HasOne(d => d.St)
                    .WithMany(p => p.TrnInventory)
                    .HasForeignKey(d => d.Stid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnInventory_TrnStockTransfer");
            });

            modelBuilder.Entity<TrnJournal>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnJournal_MstAccount");

                entity.HasOne(d => d.Aprr)
                    .WithMany(p => p.TrnJournalAprr)
                    .HasForeignKey(d => d.Aprrid)
                    .HasConstraintName("FK_TrnJournal_TrnReceivingReceipt1");

                entity.HasOne(d => d.Arsi)
                    .WithMany(p => p.TrnJournalArsi)
                    .HasForeignKey(d => d.Arsiid)
                    .HasConstraintName("FK_TrnJournal_TrnSalesInvoice1");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnJournal_MstArticle");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnJournal_MstBranch");

                entity.HasOne(d => d.Cv)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.Cvid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnDisbursement");

                entity.HasOne(d => d.In)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.Inid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnStockIn");

                entity.HasOne(d => d.Jv)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.Jvid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnJournalVoucher");

                entity.HasOne(d => d.Or)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.Orid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnCollection");

                entity.HasOne(d => d.Ot)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.Otid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnStockOut");

                entity.HasOne(d => d.Rr)
                    .WithMany(p => p.TrnJournalRr)
                    .HasForeignKey(d => d.Rrid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnReceivingReceipt");

                entity.HasOne(d => d.Si)
                    .WithMany(p => p.TrnJournalSi)
                    .HasForeignKey(d => d.Siid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnSalesInvoice");

                entity.HasOne(d => d.St)
                    .WithMany(p => p.TrnJournal)
                    .HasForeignKey(d => d.Stid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_TrnJournal_TrnStockTransfer");
            });

            modelBuilder.Entity<TrnJournalVoucherLine>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TrnJournalVoucherLine)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnJournalVoucherLine_MstAccount");

                entity.HasOne(d => d.Aprr)
                    .WithMany(p => p.TrnJournalVoucherLine)
                    .HasForeignKey(d => d.Aprrid)
                    .HasConstraintName("FK_TrnJournalVoucherLine_TrnReceivingReceipt");

                entity.HasOne(d => d.Arsi)
                    .WithMany(p => p.TrnJournalVoucherLine)
                    .HasForeignKey(d => d.Arsiid)
                    .HasConstraintName("FK_TrnJournalVoucherLine_TrnSalesInvoice");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TrnJournalVoucherLine)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnJournalVoucherLine_MstArticle");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnJournalVoucherLine)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnJournalVoucherLine_MstBranch");

                entity.HasOne(d => d.Jv)
                    .WithMany(p => p.TrnJournalVoucherLine)
                    .HasForeignKey(d => d.Jvid)
                    .HasConstraintName("FK_TrnJournalVoucherLine_TrnJournalVoucher");
            });

            modelBuilder.Entity<TrnPurchaseOrder>(entity =>
            {
                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnPurchaseOrder)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnPurchaseOrder_MstBranch");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.TrnPurchaseOrder)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnPurchaseOrder_MstArticle");

                entity.HasOne(d => d.Term)
                    .WithMany(p => p.TrnPurchaseOrder)
                    .HasForeignKey(d => d.TermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnPurchaseOrder_MstTerm");
            });

            modelBuilder.Entity<TrnPurchaseOrderItem>(entity =>
            {
                entity.HasOne(d => d.Item)
                    .WithMany(p => p.TrnPurchaseOrderItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnPurchaseOrderItem_MstArticle");

                entity.HasOne(d => d.Po)
                    .WithMany(p => p.TrnPurchaseOrderItem)
                    .HasForeignKey(d => d.Poid)
                    .HasConstraintName("FK_TrnPurchaseOrderItem_TrnPurchaseOrder");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.TrnPurchaseOrderItem)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnPurchaseOrderItem_MstUnit");
            });

            modelBuilder.Entity<TrnReceivingReceipt>(entity =>
            {
                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnReceivingReceipt)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceipt_MstBranch");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.TrnReceivingReceipt)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceipt_MstArticle");

                entity.HasOne(d => d.Term)
                    .WithMany(p => p.TrnReceivingReceipt)
                    .HasForeignKey(d => d.TermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceipt_MstTerm");
            });

            modelBuilder.Entity<TrnReceivingReceiptItem>(entity =>
            {
                entity.HasOne(d => d.BaseUnit)
                    .WithMany(p => p.TrnReceivingReceiptItemBaseUnit)
                    .HasForeignKey(d => d.BaseUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_MstUnit1");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnReceivingReceiptItem)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_MstBranch");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.TrnReceivingReceiptItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_MstArticle");

                entity.HasOne(d => d.Po)
                    .WithMany(p => p.TrnReceivingReceiptItem)
                    .HasForeignKey(d => d.Poid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_TrnPurchaseOrder");

                entity.HasOne(d => d.Rr)
                    .WithMany(p => p.TrnReceivingReceiptItem)
                    .HasForeignKey(d => d.Rrid)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_TrnReceivingReceipt");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.TrnReceivingReceiptItemUnit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_MstUnit");

                entity.HasOne(d => d.Vat)
                    .WithMany(p => p.TrnReceivingReceiptItemVat)
                    .HasForeignKey(d => d.Vatid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_MstTaxType");

                entity.HasOne(d => d.Wtax)
                    .WithMany(p => p.TrnReceivingReceiptItemWtax)
                    .HasForeignKey(d => d.Wtaxid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnReceivingReceiptItem_MstTaxType1");
            });

            modelBuilder.Entity<TrnSalesInvoice>(entity =>
            {
                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnSalesInvoice)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoice_MstBranch");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.TrnSalesInvoice)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoice_MstArticle");

                entity.HasOne(d => d.Term)
                    .WithMany(p => p.TrnSalesInvoice)
                    .HasForeignKey(d => d.TermId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoice_MstTerm");
            });

            modelBuilder.Entity<TrnSalesInvoiceItem>(entity =>
            {
                entity.HasOne(d => d.BaseUnit)
                    .WithMany(p => p.TrnSalesInvoiceItemBaseUnit)
                    .HasForeignKey(d => d.BaseUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoiceItem_MstUnit1");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.TrnSalesInvoiceItem)
                    .HasForeignKey(d => d.DiscountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoiceItem_MstDiscount");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.TrnSalesInvoiceItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoiceItem_MstArticle");

                entity.HasOne(d => d.ItemInventory)
                    .WithMany(p => p.TrnSalesInvoiceItem)
                    .HasForeignKey(d => d.ItemInventoryId)
                    .HasConstraintName("FK_TrnSalesInvoiceItem_MstArticleInventory");

                entity.HasOne(d => d.Si)
                    .WithMany(p => p.TrnSalesInvoiceItem)
                    .HasForeignKey(d => d.Siid)
                    .HasConstraintName("FK_TrnSalesInvoiceItem_TrnSalesInvoice");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.TrnSalesInvoiceItemUnit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoiceItem_MstUnit");

                entity.HasOne(d => d.Vat)
                    .WithMany(p => p.TrnSalesInvoiceItem)
                    .HasForeignKey(d => d.Vatid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnSalesInvoiceItem_MstTaxType");
            });

            modelBuilder.Entity<TrnStockCount>(entity =>
            {
                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnStockCount)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockCount_MstBranch");
            });

            modelBuilder.Entity<TrnStockCountItem>(entity =>
            {
                entity.HasOne(d => d.Item)
                    .WithMany(p => p.TrnStockCountItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockCountItem_MstArticle");

                entity.HasOne(d => d.Sc)
                    .WithMany(p => p.TrnStockCountItem)
                    .HasForeignKey(d => d.Scid)
                    .HasConstraintName("FK_TrnStockCountItem_TrnStockCount");
            });

            modelBuilder.Entity<TrnStockIn>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TrnStockIn)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockIn_MstAccount");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TrnStockIn)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockIn_MstArticle");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnStockIn)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockIn_MstBranch");
            });

            modelBuilder.Entity<TrnStockInItem>(entity =>
            {
                entity.HasOne(d => d.BaseUnit)
                    .WithMany(p => p.TrnStockInItemBaseUnit)
                    .HasForeignKey(d => d.BaseUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockInItem_MstUnit1");

                entity.HasOne(d => d.In)
                    .WithMany(p => p.TrnStockInItem)
                    .HasForeignKey(d => d.Inid)
                    .HasConstraintName("FK_TrnStockInItem_TrnStockIn");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.TrnStockInItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockInItem_MstArticle");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.TrnStockInItemUnit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockInItem_MstUnit");
            });

            modelBuilder.Entity<TrnStockOut>(entity =>
            {
                entity.HasOne(d => d.Account)
                    .WithMany(p => p.TrnStockOut)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOut_MstAccount");

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.TrnStockOut)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOut_MstArticle");

                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnStockOut)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOut_MstBranch");
            });

            modelBuilder.Entity<TrnStockOutItem>(entity =>
            {
                entity.HasOne(d => d.BaseUnit)
                    .WithMany(p => p.TrnStockOutItemBaseUnit)
                    .HasForeignKey(d => d.BaseUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOutItem_MstUnit1");

                entity.HasOne(d => d.ExpenseAccount)
                    .WithMany(p => p.TrnStockOutItem)
                    .HasForeignKey(d => d.ExpenseAccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOutItem_MstAccount");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.TrnStockOutItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOutItem_MstArticle");

                entity.HasOne(d => d.ItemInventory)
                    .WithMany(p => p.TrnStockOutItem)
                    .HasForeignKey(d => d.ItemInventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOutItem_MstArticleInventory");

                entity.HasOne(d => d.Ot)
                    .WithMany(p => p.TrnStockOutItem)
                    .HasForeignKey(d => d.Otid)
                    .HasConstraintName("FK_TrnStockOutItem_TrnStockOut");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.TrnStockOutItemUnit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockOutItem_MstUnit");
            });

            modelBuilder.Entity<TrnStockTransfer>(entity =>
            {
                entity.HasOne(d => d.Branch)
                    .WithMany(p => p.TrnStockTransferBranch)
                    .HasForeignKey(d => d.BranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockTransfer_MstBranch");

                entity.HasOne(d => d.ToBranch)
                    .WithMany(p => p.TrnStockTransferToBranch)
                    .HasForeignKey(d => d.ToBranchId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockTransfer_MstBranch1");
            });

            modelBuilder.Entity<TrnStockTransferItem>(entity =>
            {
                entity.HasOne(d => d.BaseUnit)
                    .WithMany(p => p.TrnStockTransferItemBaseUnit)
                    .HasForeignKey(d => d.BaseUnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockTransferItem_MstUnit1");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.TrnStockTransferItem)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockTransferItem_MstArticle");

                entity.HasOne(d => d.ItemInventory)
                    .WithMany(p => p.TrnStockTransferItem)
                    .HasForeignKey(d => d.ItemInventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockTransferItem_MstArticleInventory");

                entity.HasOne(d => d.St)
                    .WithMany(p => p.TrnStockTransferItem)
                    .HasForeignKey(d => d.Stid)
                    .HasConstraintName("FK_TrnStockTransferItem_TrnStockTransfer");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.TrnStockTransferItemUnit)
                    .HasForeignKey(d => d.UnitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrnStockTransferItem_MstUnit");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
