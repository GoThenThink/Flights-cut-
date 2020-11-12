using FluentMigrator;

namespace Migrations
{
    [Migration(2020062808, "Created table flights")]
    public sealed class _2020062808_CrTabFlights : Migration
    {
        public override void Down()
        {
            Delete.Table(FlightsConst.Table);
        }

        public override void Up()
        {
            Create.Table(FlightsConst.Table)
                .WithColumn(FlightsConst.Id).AsInt64().PrimaryKey().Identity()
                .WithColumn(FlightsConst.Name).AsAnsiString(256).NotNullable()
                .WithColumn(FlightsConst.From).AsInt64().NotNullable().ForeignKey(FlightsConst.FkFrom, AirportsConst.Table, AirportsConst.Id)
                .WithColumn(FlightsConst.To).AsInt64().NotNullable().ForeignKey(FlightsConst.FkTo, AirportsConst.Table, AirportsConst.Id)
                .WithColumn(FlightsConst.AirlineId).AsInt32().NotNullable().ForeignKey(FlightsConst.FkAirline, AirlinesConst.Table, AirlinesConst.Id)
                .WithColumn(FlightsConst.FlightTypeId).AsInt16().NotNullable().ForeignKey(FlightsConst.FkFlightType, FlightTypesConst.Table, FlightTypesConst.Id)
                .WithColumn(FlightsConst.TouristInfoEditLock).AsInt32().Nullable()
                .WithColumn(FlightsConst.ReturnFlightSameAirlineRequired).AsBoolean().NotNullable()
                .WithColumn(FlightsConst.AlternativeSearch).AsBoolean().NotNullable()
                .WithColumn(FlightsConst.FlightDuration).AsInt32().NotNullable()
                .WithColumn(FlightsConst.AirplaneModelId).AsInt32().NotNullable().ForeignKey(FlightsConst.FkAirplaneModel, AirplaneModelsConst.Table, AirplaneModelsConst.Id)
                .WithColumn(FlightsConst.StatusId).AsInt16().NotNullable().ForeignKey(FlightsConst.FkStatus, FlightStatusConst.Table, FlightStatusConst.Id);

            Create.Index(FlightsConst.IxPk)
                .OnTable(FlightsConst.Table).WithOptions().Clustered().OnColumn(FlightsConst.Id);
            Create.Index(FlightsConst.IxName)
                .OnTable(FlightsConst.Table).WithOptions().NonClustered().OnColumn(FlightsConst.Name).Ascending();
            Create.Index(FlightsConst.IxFrom)
                .OnTable(FlightsConst.Table).WithOptions().NonClustered().OnColumn(FlightsConst.From).Ascending();
            Create.Index(FlightsConst.IxTo)
                .OnTable(FlightsConst.Table).WithOptions().NonClustered().OnColumn(FlightsConst.To).Ascending();
        }
    }
}
