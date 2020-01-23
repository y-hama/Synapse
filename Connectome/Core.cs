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

            public TimeSpan ProcessTime { get; set; }
            public int ProcessFrame { get; set; }
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
            CoreObjects.Count = CoreObjects.Cells.Count;

            for (int i = 0; i < CoreObjects.Count; i++)
            {
                if (CoreObjects.Cells[i].Type == Field.Domain.CellInfomation.CellType.Synapse)
                {
                    var cell = CoreObjects.Cells[i];
                    for (int j = 0; j < CoreObjects.Count; j++)
                    {
                        if (cell.Location.DistanceTo(CoreObjects.Cells[j].Location) < cell.AxsonLength)
                        {
                            cell.ConnectedCells.Add(CoreObjects.Cells[j]);
                        }
                    }
                }
            }


            CoreObjects.Infomation.Value = new Components.RNdArray(CoreObjects.Count);
            CoreObjects.Infomation.Signal = new Components.RNdArray(CoreObjects.Count);
            CoreObjects.Infomation.Potential = new Components.RNdArray(CoreObjects.Count);
            CoreObjects.Infomation.Activity = new Components.RNdArray(CoreObjects.Count);

            CoreObjects.Infomation.Weight = new Components.RNdArray(1);
            CoreObjects.Infomation.ConnectionCount = new Components.RNdArray(1);
            CoreObjects.Infomation.ConnectionIndex = new Components.RNdArray(1);
            CoreObjects.Infomation.ConnectionStartPosition = new Components.RNdArray(1);

            CreateInfomation();
        }

        public static void Start()
        {
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
                        for (int i = 0; i < CoreObjects.Count; i++)
                        {
                            CoreObjects.Cells[i].Value = CoreObjects.Infomation.Value[i];
                            CoreObjects.Cells[i].Signal = CoreObjects.Infomation.Signal[i];
                            CoreObjects.Cells[i].Potential = CoreObjects.Infomation.Potential[i];
                            CoreObjects.Cells[i].Activity = CoreObjects.Infomation.Activity[i];
                        }
                        //Parallel.For(0, CoreObjects.Count, new ParallelOptions()
                        //{
                        //    MaxDegreeOfParallelism = 1,
                        //}, i =>
                        //{
                        //    CoreObjects.Cells[i].Value = CoreObjects.Infomation.Value[i];
                        //    CoreObjects.Cells[i].Signal = CoreObjects.Infomation.Signal[i];
                        //    CoreObjects.Cells[i].Potential = CoreObjects.Infomation.Potential[i];
                        //    CoreObjects.Cells[i].Activity = CoreObjects.Infomation.Activity[i];
                        //});

                        var e = new DataUploadEventArgs();
                        e.AxonConnection = new DataUploadEventArgs.AxonConnectionIndex()
                        {
                            Average = CoreObjects.AxsonConnectionAverage,
                            Max = CoreObjects.AxsonConnectionMax,
                            Min = CoreObjects.AxsonConnectionMin,
                        };
                        e.AreaCorner = CoreObjects.AreaCorner;
                        foreach (var cell in CoreObjects.Cells)
                        {
                            e.Infomations.Add(cell.Clone());
                        }
                        e.ProcessTime = DateTime.Now - CoreObjects.LatestSequenceTime;
                        e.ProcessFrame = CoreObjects.MaxStepOverCount - CoreObjects.LatestSequenceIndex;
                        DataUploadHandler?.Invoke(e);
                        CoreObjects.LatestSequenceTime = DateTime.Now;
                        CoreObjects.LatestSequenceIndex = CoreObjects.MaxStepOverCount;

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
            int cnt = 200;

            AddField(new Field.Receptor(new Field.Domain.Sensor.RandomPulser(
                new Location(4, 0, 0), 1, cnt)));

            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(4, 0, 0), 2, cnt, 8)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(0, 0, 0), 5, cnt, 16)));
        }
    }
}
