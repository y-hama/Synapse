using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome
{
    public static class Core
    {
        public static void Terminate()
        {
            CoreObjects.IsTerminated = true;
        }

        public class DataUploadEventArgs
        {
            public class AxonConnectionIndex
            {
                public double Max { get; set; }
                public double Min { get; set; }
                public double Average { get; set; }
            }

            public List<Field.Domain.CellInfomation> Infomations { get; set; } = new List<Field.Domain.CellInfomation>();
            public Location.LocationSet AreaCorner { get; set; }

            public AxonConnectionIndex AxonConnection { get; set; }
        }

        public delegate void DataUploadEventHandler(DataUploadEventArgs e);
        private static DataUploadEventHandler DataUploadHandler { get; set; }
        public static event DataUploadEventHandler DataUpload
        {
            add { DataUploadHandler += value; }
            remove { DataUploadHandler -= value; }
        }
        private static bool DataCreating { get; set; } = false;


        private static void CatchException(Exception ex)
        {
            throw ex;
        }

        private static Location.LocationSet AreaCorner()
        {
            Location min = new Location(), max = new Location();
            for (int i = 0; i < CoreObjects.Fields.Count; i++)
            {
                min.X = Math.Min(min.X, CoreObjects.Fields[i].Domain.AreaMinState.X);
                min.Y = Math.Min(min.Y, CoreObjects.Fields[i].Domain.AreaMinState.Y);
                min.Z = Math.Min(min.Z, CoreObjects.Fields[i].Domain.AreaMinState.Z);
                max.X = Math.Max(max.X, CoreObjects.Fields[i].Domain.AreaMaxState.X);
                max.Y = Math.Max(max.Y, CoreObjects.Fields[i].Domain.AreaMaxState.Y);
                max.Z = Math.Max(max.Z, CoreObjects.Fields[i].Domain.AreaMaxState.Z);
            }

            return new Connectome.Location.LocationSet(min, max);
        }

        public static void Initialize()
        {
            Components.State.CatchException = CatchException;
            Components.State.SetAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            Components.State.AddSharedSourceGroup("Connectome.Gpgpu.Shared");
            Components.State.AddSourceGroup("Connectome.Gpgpu.Function");
            Components.State.Initialize();

            CreateConnectome();
        }

        private static void AddField(Field.FieldCore field)
        {
            CoreObjects.Fields.Add(field);
        }

        private static void ConfirmField()
        {
            CoreObjects.AreaCorner = AreaCorner();

            foreach (var field in CoreObjects.Fields)
            {
                foreach (var cell in field.Domain.Cells)
                {
                    CoreObjects.Cells.Add(cell);
                }
            }
            CoreObjects.Cells.Sort((x, y) =>
            {
                if (x.ID > y.ID) { return 1; }
                else
                if (x.ID < y.ID) { return -1; }
                else { return 0; }
            });

            foreach (var field in CoreObjects.Fields)
            {
                var fieldlist = CoreObjects.Fields.FindAll(x => x != field);
                field.CreateConnection(fieldlist);
            }

            List<double> avelist = new List<double>();
            List<double> maxlist = new List<double>();
            List<double> minlist = new List<double>();
            foreach (var field in CoreObjects.Fields)
            {
                avelist.Add(field.Domain.Cells.Average(x => x.ConnectedCells.Count));
                maxlist.Add(field.Domain.Cells.Max(x => x.ConnectedCells.Count));
                minlist.Add(field.Domain.Cells.Min(x => x.ConnectedCells.Count));
            }
            CoreObjects.AxsonConnectionAverage = avelist.Average();
            CoreObjects.AxsonConnectionMax = maxlist.Max();
            CoreObjects.AxsonConnectionMin = minlist.Min();
        }

        public static void Start()
        {
            ConfirmField();
            foreach (var item in CoreObjects.Fields)
            {
                item.Start();
            }
        }

        public static void RequestLatestState()
        {
            if (DataUploadHandler != null)
            {
                if (!DataCreating)
                {
                    DataCreating = true;
                    new System.Threading.Thread(() =>
                    {
                        var e = new DataUploadEventArgs();
                        e.AxonConnection = new DataUploadEventArgs.AxonConnectionIndex()
                        {
                            Average = CoreObjects.AxsonConnectionAverage,
                            Max = CoreObjects.AxsonConnectionMax,
                            Min = CoreObjects.AxsonConnectionMin,
                        };
                        e.AreaCorner = CoreObjects.AreaCorner;
                        foreach (var field in CoreObjects.Fields)
                        {
                            foreach (var cell in field.Domain.Cells)
                            {
                                e.Infomations.Add(cell.Clone());
                            }
                        }
                        DataUploadHandler?.Invoke(e);
                        DataCreating = false;
                    }).Start();
                }
            }
        }


        private static void CreateConnectome()
        {
            AddField(new Field.Receptor(new Field.Domain.Sensor.RandomPulsar(
                new Connectome.Location(0, 0, 0), 0.5, 1000
                )));

            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(0, 0, 0), 1, 1000, 0.1
                )));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(1.5, 0, 0), 1, 1000, 0.1
                )));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(0, 1.5, 0), 1, 1000, 0.1
                )));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(0, 0, 1.5), 1, 1000, 0.1
                )));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(-1.5, 0, 0), 1, 1000, 0.1
                )));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(0, -1.5, 0), 1, 1000, 0.1
                )));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(0, 0, -1.5), 1, 1000, 0.1
                )));

            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(1.25, 1.25, 1.25), 1.5, 1000, 0.2
                )));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Connectome.Location(-1.25, -1.25, -1.25), 1.5, 1000, 0.2
                )));
        }
    }
}
