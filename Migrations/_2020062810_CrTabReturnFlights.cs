using FluentMigrator;

namespace Migrations
{
    [Migration(2020062810, "Created table return_flights")]
    public sealed class _2020062810_CrTabReturnFlights : Migration
    {
        public override void Down()
        {
            Delete.Table(ReturnFlightsConst.Table);
        }

        public override void Up()
        {
            Create.Table(ReturnFlightsConst.Table)
                .WithColumn(ReturnFlightsConst.FlightToId).AsInt64().ForeignKey(ReturnFlightsConst.FkFlightTo, FlightsConst.Table, FlightsConst.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn(ReturnFlightsConst.FlightFromId).AsInt64().ForeignKey(ReturnFlightsConst.FkFlightFrom, FlightsConst.Table, FlightsConst.Id).OnDelete(System.Data.Rule.Cascade)
                .WithColumn(ReturnFlightsConst.Priority).AsInt16().NotNullable()
                .WithColumn(ReturnFlightsConst.IsAvailable).AsBoolean().NotNullable()
                .WithColumn(ReturnFlightsConst.IsNativeReturnFlight).AsBoolean().NotNullable();

            Create.PrimaryKey()
                .OnTable(ReturnFlightsConst.Table)
                .Columns(ReturnFlightsConst.FlightToId, ReturnFlightsConst.FlightFromId);

            Create.Index(ReturnFlightsConst.IxPk)
                .OnTable(ReturnFlightsConst.Table)
                .WithOptions()
                .Clustered()
                .OnColumn(ReturnFlightsConst.FlightToId).Ascending()
                .OnColumn(ReturnFlightsConst.FlightFromId).Ascending();    
        }
    }
}
