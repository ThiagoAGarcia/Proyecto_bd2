using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using api.Models;

namespace api.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrador> Administradors { get; set; }

    public virtual DbSet<Dispositivo> Dispositivos { get; set; }

    public virtual DbSet<Dispositivofuncionario> Dispositivofuncionarios { get; set; }

    public virtual DbSet<Entradum> Entrada { get; set; }

    public virtual DbSet<Esasignado> Esasignados { get; set; }

    public virtual DbSet<Estadio> Estadios { get; set; }

    public virtual DbSet<Etapa> Etapas { get; set; }

    public virtual DbSet<Funcionario> Funcionarios { get; set; }

    public virtual DbSet<Grupo> Grupos { get; set; }

    public virtual DbSet<Jurisdiccion> Jurisdiccions { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Pai> Pais { get; set; }

    public virtual DbSet<Partido> Partidos { get; set; }

    public virtual DbSet<Perfil> Perfils { get; set; }

    public virtual DbSet<Sector> Sectors { get; set; }

    public virtual DbSet<Telefono> Telefonos { get; set; }

    public virtual DbSet<Transferencium> Transferencia { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Ventum> Venta { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Administrador>(entity =>
        {
            entity.HasKey(e => e.MailPerfil).HasName("PRIMARY");

            entity.ToTable("administrador");

            entity.Property(e => e.MailPerfil)
                .HasMaxLength(200)
                .HasColumnName("mailPerfil");
            entity.Property(e => e.FechaAsignacionCargo).HasColumnName("fechaAsignacionCargo");

            entity.HasOne(d => d.MailPerfilNavigation).WithOne(p => p.Administrador)
                .HasForeignKey<Administrador>(d => d.MailPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("administrador_ibfk_1");
        });

        modelBuilder.Entity<Dispositivo>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("dispositivo");

            entity.Property(e => e.Identificador)
                .ValueGeneratedNever()
                .HasColumnName("identificador");
        });

        modelBuilder.Entity<Dispositivofuncionario>(entity =>
        {
            entity.HasKey(e => new { e.MailFuncionario, e.IdentificadorDispositivo })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("dispositivofuncionario");

            entity.HasIndex(e => e.IdentificadorDispositivo, "identificadorDispositivo");

            entity.Property(e => e.MailFuncionario)
                .HasMaxLength(200)
                .HasColumnName("mailFuncionario");
            entity.Property(e => e.IdentificadorDispositivo).HasColumnName("identificadorDispositivo");
            entity.Property(e => e.Fecha).HasColumnName("fecha");

            entity.HasOne(d => d.IdentificadorDispositivoNavigation).WithMany(p => p.Dispositivofuncionarios)
                .HasForeignKey(d => d.IdentificadorDispositivo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dispositivofuncionario_ibfk_2");

            entity.HasOne(d => d.MailFuncionarioNavigation).WithMany(p => p.Dispositivofuncionarios)
                .HasForeignKey(d => d.MailFuncionario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("dispositivofuncionario_ibfk_1");
        });

        modelBuilder.Entity<Entradum>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("entrada");

            entity.HasIndex(e => e.MailUsuarioTiene, "MailUsuarioTiene");

            entity.HasIndex(e => e.IdentificadorDispositivo, "identificadorDispositivo");

            entity.HasIndex(e => new { e.IdentificadorEstadio, e.IdentificadorSector }, "identificadorEstadio");

            entity.HasIndex(e => e.IdentificadorPartido, "identificadorPartido");

            entity.HasIndex(e => e.MailFuncionario, "mailFuncionario");

            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.CodigoQraceptado)
                .HasMaxLength(200)
                .HasColumnName("codigoQRAceptado");
            entity.Property(e => e.EstadoEntrada)
                .HasColumnType("enum('Registrada','No registrada','Cancelada')")
                .HasColumnName("estadoEntrada");
            entity.Property(e => e.FechaHoraIngreso)
                .HasColumnType("datetime")
                .HasColumnName("fechaHoraIngreso");
            entity.Property(e => e.IdentificadorDispositivo).HasColumnName("identificadorDispositivo");
            entity.Property(e => e.IdentificadorEstadio).HasColumnName("identificadorEstadio");
            entity.Property(e => e.IdentificadorPartido).HasColumnName("identificadorPartido");
            entity.Property(e => e.IdentificadorSector).HasColumnName("identificadorSector");
            entity.Property(e => e.IdentificadorVenta).HasColumnName("identificadorVenta");
            entity.Property(e => e.MailFuncionario)
                .HasMaxLength(200)
                .HasColumnName("mailFuncionario");
            entity.Property(e => e.MailUsuarioTiene).HasMaxLength(200);

            entity.HasOne(d => d.IdentificadorDispositivoNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.IdentificadorDispositivo)
                .HasConstraintName("entrada_ibfk_5");

            entity.HasOne(d => d.IdentificadorPartidoNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.IdentificadorPartido)
                .HasConstraintName("entrada_ibfk_2");

            entity.HasOne(d => d.MailFuncionarioNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.MailFuncionario)
                .HasConstraintName("entrada_ibfk_4");

            entity.HasOne(d => d.MailUsuarioTieneNavigation).WithMany(p => p.Entrada)
                .HasForeignKey(d => d.MailUsuarioTiene)
                .HasConstraintName("entrada_ibfk_1");

            entity.HasOne(d => d.Sector).WithMany(p => p.Entrada)
                .HasForeignKey(d => new { d.IdentificadorEstadio, d.IdentificadorSector })
                .HasConstraintName("entrada_ibfk_3");
        });

        modelBuilder.Entity<Esasignado>(entity =>
        {
            entity.HasKey(e => new { e.MailFuncionario, e.IdentificadorSector, e.IdentificadorEstadio })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("esasignado");

            entity.HasIndex(e => new { e.IdentificadorEstadio, e.IdentificadorSector }, "identificadorEstadio");

            entity.Property(e => e.MailFuncionario)
                .HasMaxLength(200)
                .HasColumnName("mailFuncionario");
            entity.Property(e => e.IdentificadorSector).HasColumnName("identificadorSector");
            entity.Property(e => e.IdentificadorEstadio).HasColumnName("identificadorEstadio");
            entity.Property(e => e.Fecha).HasColumnName("fecha");

            entity.HasOne(d => d.MailFuncionarioNavigation).WithMany(p => p.Esasignados)
                .HasForeignKey(d => d.MailFuncionario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("esasignado_ibfk_1");

            entity.HasOne(d => d.Sector).WithMany(p => p.Esasignados)
                .HasForeignKey(d => new { d.IdentificadorEstadio, d.IdentificadorSector })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("esasignado_ibfk_2");
        });

        modelBuilder.Entity<Estadio>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("estadio");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.HasIndex(e => e.NombreJurisdiccion, "nombreJurisdiccion");

            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.DireccionCalle)
                .HasMaxLength(20)
                .HasColumnName("direccionCalle");
            entity.Property(e => e.DireccionCodigoPostal).HasColumnName("direccionCodigoPostal");
            entity.Property(e => e.DireccionLocalidad)
                .HasMaxLength(200)
                .HasColumnName("direccionLocalidad");
            entity.Property(e => e.DireccionNumero).HasColumnName("direccionNumero");
            entity.Property(e => e.Nombre)
                .HasMaxLength(32)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreJurisdiccion)
                .HasMaxLength(20)
                .HasColumnName("nombreJurisdiccion");

            entity.HasOne(d => d.NombreJurisdiccionNavigation).WithMany(p => p.Estadios)
                .HasForeignKey(d => d.NombreJurisdiccion)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("estadio_ibfk_1");
        });

        modelBuilder.Entity<Etapa>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("etapa");

            entity.HasIndex(e => e.IdentificadorGrupo, "identificadorGrupo");

            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.IdentificadorGrupo).HasColumnName("identificadorGrupo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");

            entity.HasOne(d => d.IdentificadorGrupoNavigation).WithMany(p => p.Etapas)
                .HasForeignKey(d => d.IdentificadorGrupo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("etapa_ibfk_1");
        });

        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.HasKey(e => e.MailPerfil).HasName("PRIMARY");

            entity.ToTable("funcionario");

            entity.HasIndex(e => e.NumeroLegajo, "numeroLegajo").IsUnique();

            entity.Property(e => e.MailPerfil)
                .HasMaxLength(200)
                .HasColumnName("mailPerfil");
            entity.Property(e => e.NumeroLegajo).HasColumnName("numeroLegajo");

            entity.HasOne(d => d.MailPerfilNavigation).WithOne(p => p.Funcionario)
                .HasForeignKey<Funcionario>(d => d.MailPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("funcionario_ibfk_1");
        });

        modelBuilder.Entity<Grupo>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("grupo");

            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.Nombre)
                .HasMaxLength(200)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<Jurisdiccion>(entity =>
        {
            entity.HasKey(e => e.Nombre).HasName("PRIMARY");

            entity.ToTable("jurisdiccion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
            entity.Property(e => e.Continente)
                .HasMaxLength(30)
                .HasColumnName("continente");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.MailPerfil).HasName("PRIMARY");

            entity.ToTable("login");

            entity.Property(e => e.MailPerfil)
                .HasMaxLength(200)
                .HasColumnName("mailPerfil");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .HasColumnName("password");

            entity.HasOne(d => d.MailPerfilNavigation).WithOne(p => p.Login)
                .HasForeignKey<Login>(d => d.MailPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("login_ibfk_1");
        });

        modelBuilder.Entity<Pai>(entity =>
        {
            entity.HasKey(e => e.Nombre).HasName("PRIMARY");

            entity.ToTable("pais");

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .HasColumnName("nombre");
            entity.Property(e => e.Continente)
                .HasMaxLength(30)
                .HasColumnName("continente");

            entity.HasMany(d => d.IdentificadorGrupos).WithMany(p => p.NombrePais)
                .UsingEntity<Dictionary<string, object>>(
                    "Pertenece",
                    r => r.HasOne<Grupo>().WithMany()
                        .HasForeignKey("IdentificadorGrupo")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("pertenece_ibfk_2"),
                    l => l.HasOne<Pai>().WithMany()
                        .HasForeignKey("NombrePais")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("pertenece_ibfk_1"),
                    j =>
                    {
                        j.HasKey("NombrePais", "IdentificadorGrupo")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("pertenece");
                        j.HasIndex(new[] { "IdentificadorGrupo" }, "identificadorGrupo");
                        j.IndexerProperty<string>("NombrePais")
                            .HasMaxLength(30)
                            .HasColumnName("nombrePais");
                        j.IndexerProperty<int>("IdentificadorGrupo").HasColumnName("identificadorGrupo");
                    });
        });

        modelBuilder.Entity<Partido>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("partido");

            entity.HasIndex(e => e.IdentificadorEstadio, "identificadorEstadio");

            entity.HasIndex(e => e.PaisLocal, "paisLocal");

            entity.HasIndex(e => e.PaisVisitante, "paisVisitante");

            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.Fase)
                .HasMaxLength(30)
                .HasColumnName("fase");
            entity.Property(e => e.FechaHora)
                .HasColumnType("datetime")
                .HasColumnName("fechaHora");
            entity.Property(e => e.IdentificadorEstadio).HasColumnName("identificadorEstadio");
            entity.Property(e => e.PaisLocal)
                .HasMaxLength(32)
                .HasColumnName("paisLocal");
            entity.Property(e => e.PaisVisitante)
                .HasMaxLength(32)
                .HasColumnName("paisVisitante");

            entity.HasOne(d => d.IdentificadorEstadioNavigation).WithMany(p => p.Partidos)
                .HasForeignKey(d => d.IdentificadorEstadio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partido_ibfk_3");

            entity.HasOne(d => d.PaisLocalNavigation).WithMany(p => p.PartidoPaisLocalNavigations)
                .HasForeignKey(d => d.PaisLocal)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partido_ibfk_1");

            entity.HasOne(d => d.PaisVisitanteNavigation).WithMany(p => p.PartidoPaisVisitanteNavigations)
                .HasForeignKey(d => d.PaisVisitante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("partido_ibfk_2");

            entity.HasMany(d => d.MailAdministradors).WithMany(p => p.IdentificadorPartidos)
                .UsingEntity<Dictionary<string, object>>(
                    "Gestiona",
                    r => r.HasOne<Administrador>().WithMany()
                        .HasForeignKey("MailAdministrador")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("gestiona_ibfk_2"),
                    l => l.HasOne<Partido>().WithMany()
                        .HasForeignKey("IdentificadorPartido")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("gestiona_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdentificadorPartido", "MailAdministrador")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("gestiona");
                        j.HasIndex(new[] { "MailAdministrador" }, "mailAdministrador");
                        j.IndexerProperty<int>("IdentificadorPartido")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("identificadorPartido");
                        j.IndexerProperty<string>("MailAdministrador")
                            .HasMaxLength(200)
                            .HasColumnName("mailAdministrador");
                    });
        });

        modelBuilder.Entity<Perfil>(entity =>
        {
            entity.HasKey(e => e.Mail).HasName("PRIMARY");

            entity.ToTable("perfil");

            entity.HasIndex(e => e.NumeroDocumento, "numeroDocumento").IsUnique();

            entity.Property(e => e.Mail)
                .HasMaxLength(200)
                .HasColumnName("mail");
            entity.Property(e => e.DireccionCodigoPostal).HasColumnName("direccionCodigoPostal");
            entity.Property(e => e.DireccionLocalidad)
                .HasMaxLength(32)
                .HasColumnName("direccionLocalidad");
            entity.Property(e => e.DireccionNumero).HasColumnName("direccionNumero");
            entity.Property(e => e.NumeroDocumento).HasColumnName("numeroDocumento");
            entity.Property(e => e.PaisDocumento)
                .HasMaxLength(32)
                .HasColumnName("paisDocumento");
            entity.Property(e => e.TipoDocumento)
                .HasMaxLength(32)
                .HasColumnName("tipoDocumento");
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            entity.HasKey(e => new { e.IdentificadorEstadio, e.Identificador })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("sector");

            entity.Property(e => e.IdentificadorEstadio).HasColumnName("identificadorEstadio");
            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.CapMax).HasColumnName("capMax");
            entity.Property(e => e.Nombre)
                .HasMaxLength(10)
                .HasColumnName("nombre");
            entity.Property(e => e.TarifaExtra).HasColumnName("tarifaExtra");

            entity.HasOne(d => d.IdentificadorEstadioNavigation).WithMany(p => p.Sectors)
                .HasForeignKey(d => d.IdentificadorEstadio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("sector_ibfk_1");

            entity.HasMany(d => d.IdentificadorPartidos).WithMany(p => p.Sectors)
                .UsingEntity<Dictionary<string, object>>(
                    "Habilitum",
                    r => r.HasOne<Partido>().WithMany()
                        .HasForeignKey("IdentificadorPartido")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("habilita_ibfk_2"),
                    l => l.HasOne<Sector>().WithMany()
                        .HasForeignKey("IdentificadorEstadio", "IdentificadorSector")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("habilita_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdentificadorEstadio", "IdentificadorPartido", "IdentificadorSector")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
                        j.ToTable("habilita");
                        j.HasIndex(new[] { "IdentificadorEstadio", "IdentificadorSector" }, "identificadorEstadio");
                        j.HasIndex(new[] { "IdentificadorPartido" }, "identificadorPartido");
                        j.IndexerProperty<int>("IdentificadorEstadio").HasColumnName("identificadorEstadio");
                        j.IndexerProperty<int>("IdentificadorPartido").HasColumnName("identificadorPartido");
                        j.IndexerProperty<int>("IdentificadorSector").HasColumnName("identificadorSector");
                    });
        });

        modelBuilder.Entity<Telefono>(entity =>
        {
            entity.HasKey(e => e.MailPerfil).HasName("PRIMARY");

            entity.ToTable("telefono");

            entity.Property(e => e.MailPerfil)
                .HasMaxLength(200)
                .HasColumnName("mailPerfil");
            entity.Property(e => e.Telefono1).HasColumnName("telefono");

            entity.HasOne(d => d.MailPerfilNavigation).WithOne(p => p.Telefono)
                .HasForeignKey<Telefono>(d => d.MailPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("telefono_ibfk_1");
        });

        modelBuilder.Entity<Transferencium>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("transferencia");

            entity.HasIndex(e => e.IdentificadorEntrada, "identificadorEntrada");

            entity.HasIndex(e => e.MailUsuarioRealiza, "mailUsuarioRealiza");

            entity.HasIndex(e => e.MailUsuarioRecibe, "mailUsuarioRecibe");

            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.IdentificadorEntrada).HasColumnName("identificadorEntrada");
            entity.Property(e => e.MailUsuarioRealiza)
                .HasMaxLength(200)
                .HasColumnName("mailUsuarioRealiza");
            entity.Property(e => e.MailUsuarioRecibe)
                .HasMaxLength(200)
                .HasColumnName("mailUsuarioRecibe");

            entity.HasOne(d => d.IdentificadorEntradaNavigation).WithMany(p => p.Transferencia)
                .HasForeignKey(d => d.IdentificadorEntrada)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transferencia_ibfk_1");

            entity.HasOne(d => d.MailUsuarioRealizaNavigation).WithMany(p => p.TransferenciumMailUsuarioRealizaNavigations)
                .HasForeignKey(d => d.MailUsuarioRealiza)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transferencia_ibfk_2");

            entity.HasOne(d => d.MailUsuarioRecibeNavigation).WithMany(p => p.TransferenciumMailUsuarioRecibeNavigations)
                .HasForeignKey(d => d.MailUsuarioRecibe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transferencia_ibfk_3");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.MailPerfil).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.Property(e => e.MailPerfil)
                .HasMaxLength(200)
                .HasColumnName("mailPerfil");
            entity.Property(e => e.EstadoVerificado)
                .HasColumnType("enum('verificado','No verificado')")
                .HasColumnName("estadoVerificado");
            entity.Property(e => e.FechaRegistro).HasColumnName("fechaRegistro");

            entity.HasOne(d => d.MailPerfilNavigation).WithOne(p => p.Usuario)
                .HasForeignKey<Usuario>(d => d.MailPerfil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("usuario_ibfk_1");
        });

        modelBuilder.Entity<Ventum>(entity =>
        {
            entity.HasKey(e => e.Identificador).HasName("PRIMARY");

            entity.ToTable("venta");

            entity.HasIndex(e => e.MailUsuarioComprado, "mailUsuarioComprado");

            entity.Property(e => e.Identificador).HasColumnName("identificador");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.MailUsuarioComprado)
                .HasMaxLength(200)
                .HasColumnName("mailUsuarioComprado");
            entity.Property(e => e.MontoTotal).HasColumnName("montoTotal");
            entity.Property(e => e.PorcentakeComision).HasColumnName("porcentakeComision");

            entity.HasOne(d => d.MailUsuarioCompradoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.MailUsuarioComprado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("venta_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
