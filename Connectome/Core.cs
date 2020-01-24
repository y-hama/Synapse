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

        private static void CatchException(Exception ex)
        {
            throw ex;
        }

        #region Event
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
            public double FPS { get; set; }
        }

        public delegate void DataUploadEventHandler(DataUploadEventArgs e);
        private static DataUploadEventHandler DataUploadHandler { get; set; }
        public static event DataUploadEventHandler DataUpload
        {
            add { DataUploadHandler += value; }
            remove { DataUploadHandler -= value; }
        }
        private static bool DataCreating { get; set; } = false;

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
                            CoreObjects.Cells[i].State = (Field.Domain.CellInfomation.IgnitionState)Enum.ToObject(typeof(Field.Domain.CellInfomation.IgnitionState), (int)CoreObjects.Infomation.State[i]);
                            CoreObjects.Cells[i].Value = CoreObjects.Infomation.Value[i];
                            CoreObjects.Cells[i].Signal = CoreObjects.Infomation.Signal[i];
                            CoreObjects.Cells[i].Potential = CoreObjects.Infomation.Potential[i];
                            CoreObjects.Cells[i].Activity = CoreObjects.Infomation.Activity[i];
                            int start = (int)CoreObjects.Infomation.ConnectionStartPosition[i];
                            for (int j = 0; j < CoreObjects.Infomation.ConnectionCount[i]; j++)
                            {
                                CoreObjects.Cells[i].ConnectionWeight[j] = CoreObjects.Infomation.Weight[start + j];
                            }
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
                        e.ProcessFrame = 1 + (CoreObjects.MaxStepOverCount - CoreObjects.LatestSequenceIndex);
                        CoreObjects.FPS = 0.9 * CoreObjects.FPS + (1 - 0.9) * ((1000.0 / (e.ProcessTime.TotalMilliseconds / e.ProcessFrame)));
                        e.FPS = CoreObjects.FPS;
                        DataUploadHandler?.Invoke(e);
                        CoreObjects.LatestSequenceTime = DateTime.Now;
                        CoreObjects.LatestSequenceIndex = CoreObjects.MaxStepOverCount;
                        DataCreating = false;
                    }).Start();
                }
            }
        }
        #endregion

        #region ParameterSettings
        public static void SetTimeScale(int scale)
        {
            CoreObjects.TimeScale = scale;
        }
        #endregion

        #region Constructor and Initialize
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

        public static void Start()
        {
            foreach (var item in CoreObjects.Fields)
            {
                item.Start();
            }
        }
        #endregion

        #region Connectome Configuration
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

            CoreObjects.Cells = CoreObjects.Cells.OrderBy(x => Guid.NewGuid()).ToList();
            for (int i = 0; i < CoreObjects.Count; i++)
            {
                if (CoreObjects.Cells[i].Type == Field.Domain.CellInfomation.CellType.Synapse)
                {
                    var cell = CoreObjects.Cells[i];
                    for (int j = 0; j < CoreObjects.Count; j++)
                    {
                        if (cell.Location.DistanceTo(CoreObjects.Cells[j].Location) < cell.AxsonLength
                            //&& !CoreObjects.Cells[j].ConnectedCells.Contains(cell)
                            )
                        {
                            cell.ConnectedCells.Add(CoreObjects.Cells[j]);
                        }
                    }
                }
            }
            CoreObjects.Cells.Sort((x, y) =>
            {
                if (x.ID < y.ID)
                {
                    return -1;
                }
                else
                if (x.ID > y.ID)
                {
                    return 1;
                }
                else { return 0; }
            });

            CoreObjects.Infomation.State = new Components.RNdArray(CoreObjects.Count);
            for (int i = 0; i < CoreObjects.Count; i++)
            {
                CoreObjects.Infomation.State[i] = (int)CoreObjects.Cells[i].State;
            }

            CoreObjects.Infomation.Value = new Components.RNdArray(CoreObjects.Count);
            CoreObjects.Infomation.Signal = new Components.RNdArray(CoreObjects.Count);
            CoreObjects.Infomation.Potential = new Components.RNdArray(CoreObjects.Count);
            CoreObjects.Infomation.Activity = new Components.RNdArray(CoreObjects.Count);

            CoreObjects.Infomation.ConnectionStartPosition = new Components.RNdArray(CoreObjects.Count);
            CoreObjects.Infomation.ConnectionCount = new Components.RNdArray(CoreObjects.Count);
            int pos = 0, cnt = 0;
            for (int i = 0; i < CoreObjects.Count; i++)
            {
                CoreObjects.Infomation.ConnectionStartPosition[i] = pos;
                cnt = CoreObjects.Cells[i].ConnectedCells.Count;
                CoreObjects.Infomation.ConnectionCount[i] = cnt;
                pos += cnt;
            }

            CoreObjects.Infomation.Weight = new Components.RNdArray((int)CoreObjects.Infomation.ConnectionCount.Data.Sum(x => x.Value));


            CoreObjects.Infomation.ConnectionIndex = new Components.RNdArray(CoreObjects.Infomation.Weight.Length);
            int index = 0;
            for (int i = 0; i < CoreObjects.Count; i++)
            {
                double weight = 1.0 / (double)CoreObjects.Infomation.ConnectionCount[i];
                for (int j = 0; j < CoreObjects.Infomation.ConnectionCount[i]; j++)
                {
                    CoreObjects.Cells[i].ConnectionWeight.Add(weight);
                    CoreObjects.Infomation.Weight[index] = weight;
                    CoreObjects.Infomation.ConnectionIndex[index] = CoreObjects.Cells[i].ConnectedCells[j].ID;
                    index++;
                }
            }

            CreateInfomation();
        }

        private static void AddField(Field.FieldCore field)
        {
            CoreObjects.Fields.Add(field);
        }
        #endregion

        #region Connectome Creator
        private static void CreateConnectome()
        {
            int cnt = 50;

            #region RandomPulser(0,0,0) and Synapse(xy:-5<>5)
            var center1 = new Location(0, 0, 0);
            AddField(new Field.Receptor(new Field.Domain.Sensor.RandomPulser(
                center1, 0.5, 10)));

            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                center1, 2, 2 * cnt, 2)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                center1, 5, 10 * cnt, 4)));
            #endregion

            #region Synapse(6,0,0) (x:4<>13)
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(6, 0, 0), 2, 10 * cnt, 3)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(10, 0, 0), 3, 10 * cnt, 4)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(10, 5, 0), 3, 10 * cnt, 3)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(6, 9, 0), 3, 10 * cnt, 4)));
            #endregion

            #region Synapse(-6,0,0) (x:-4<>-13)
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(-6, 0, 0), 2, 10 * cnt, 3)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(-10, 0, 0), 3, 10 * cnt, 4)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(-10, 5, 0), 3, 10 * cnt, 3)));
            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(-6, 9, 0), 3, 10 * cnt, 4)));
            #endregion

            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(0, 13, 0), 5, 10 * cnt, 4)));

            AddField(new Field.Area(new Field.Domain.Transporter.SynapseConnection(
                new Location(0, 6, -7), 5, 10 * cnt, 3)));
        }
        #endregion
    }
}
