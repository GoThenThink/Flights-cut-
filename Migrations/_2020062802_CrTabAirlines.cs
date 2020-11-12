using FluentMigrator;

namespace Migrations
{
    [Migration(2020062802, "Created table airlines")]
    public sealed class _2020062802_CrTabAirlines : Migration
    {
        public override void Down()
        {
            Delete.Table(AirlinesConst.Table);
        }

        public override void Up()
        {
            Create.Table(AirlinesConst.Table)
                .WithColumn(AirlinesConst.Id).AsInt32().PrimaryKey().Identity()
                .WithColumn(AirlinesConst.Name).AsAnsiString(256).NotNullable().Unique()
                .WithColumn(AirlinesConst.Iata).AsAnsiString(4).NotNullable().Unique();

            Create.Index(AirlinesConst.IxPk)
                .OnTable(AirlinesConst.Table).WithOptions().Clustered().OnColumn(AirlinesConst.Id);
            Create.Index(AirlinesConst.IxIata)
                .OnTable(AirlinesConst.Table).WithOptions().NonClustered().OnColumn(AirlinesConst.Iata).Ascending();
        }
    }
}
