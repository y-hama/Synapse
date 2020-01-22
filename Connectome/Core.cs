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
            public Location.LocationCornerSet AreaCorner { get; set; }

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

        public static void Initialize()
        {
            Components.State.CatchException = CatchException;
            Components.State.SetAssembly(System.Reflection.Assembly.GetExecutingAssembly());
            Components.State.AddSharedSourceGroup("Connectome.Gpgpu.Shared");
            Components.State.AddSourceGroup("Connectome.Gpgpu.Function");
            Components.State.Initialize();

            CreateConnectome();
            ConfirmConnectome();
        }
        private static void CreateInfomation()
        {
            CoreObjects.AreaCorner = AreaCorner();

            double min = int.MaxValue, max = 0, ave = 0;
            foreach (var cell in CoreObjects.Cells)
            {
                var cnt = cell.ConnectedCells.Count;
                ave += cnt;
                if (min > cnt) { min = cnt; }
                if (max < cnt) { max = cnt; }
            }
            CoreObjects.AxsonConnectionAverage = ave / CoreObjects.Cells.Count;
            CoreObjects.AxsonConnectionMax = max;
            CoreObjects.AxsonConnectionMin = min;
        }

        private static Location.LocationCornerSet AreaCorner()
        {
            Location min = new Location(), max = new Location();
            for (int i = 0; i < CoreObjects.Fields.Count; i++)
            {
                min.X = Math.Min(min.X, CoreObjects.Fields[i].AreaMinState.X);
                min.Y = Math.Min(min.Y, CoreObjects.Fields[i].AreaMinState.Y);
                min.Z = Math.Min(min.Z, CoreObjects.Fields[i].AreaMinState.Z);
                max.X = Math.Max(max.X, CoreObjects.Fields[i].AreaMaxState.X);
                max.Y = Math.Max(max.Y, CoreObjects.Fields[i].AreaMaxState.Y);
                max.Z = Math.Max(max.Z, CoreObjects.Fields[i].AreaMaxState.Z);
            }

            return new Connectome.Location.LocationCornerSet(min, max);
        }

        private static void ConfirmConnectome()
        {


            //foreach (var field in CoreObjects.Fields)
            //{
            //    var fieldlist = CoreObjects.Fields.FindAll(x => x != field);
            //    field.CreateConnection(fieldlist);
            //}

            CreateInfomation();
        }

        public static void Start()
        {
            //foreach (var item in CoreObjects.Fields)
            //{
            //    item.Start();
            //}
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
                            foreach (var cell in CoreObjects.Cells)
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

        private static void AddField(Field.FieldCore field)
        {
            CoreObjects.Fields.Add(field);
        }

        private static void CreateConnectome()
        {
            int cnt = 500;
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(0, 0, 0), 4, cnt, 0.2)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(1, 0, 0), 3, cnt, 0.175)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(2, 0, 0), 2, cnt, 0.15)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(3, 0, 0), 1, cnt, 0.125)));
        }
    }
}
