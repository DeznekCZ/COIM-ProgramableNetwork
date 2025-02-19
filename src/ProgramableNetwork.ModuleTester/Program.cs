﻿using Mafi;
using Mafi.Collections.ImmutableCollections;
using Mafi.Core;
using Mafi.Core.Entities;
using Mafi.Core.Game;
using Mafi.Core.Mods;
using Mafi.Core.Products;
using Mafi.Core.Prototypes;
using ProgramableNetwork.Python;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProgramableNetwork.ModuleTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProtosDb protosDb = new ProtosDb();

            ConstructorInfo constructor = typeof(ProtoRegistrator).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();

            ProtoRegistrator registrator = (ProtoRegistrator)constructor.Invoke(new object[] { protosDb, new IConfig[0].ToImmutableArray() });
            InitBaseGame(registrator);

            Console.WriteLine(new DirectoryInfo(".").FullName);

            string[] files = new string[]
            {
                @"..\..\..\ProgramableNetwork.Modules\Custom\delay.py",
                @"..\..\..\ProgramableNetwork.Modules\Custom\connection_isactive.py",
                @"..\..\..\ProgramableNetwork.Modules\Custom\randomizer.py",
            };

            foreach (var file in files)
            {
                ModuleRegistrator.Register(registrator, file);
            }

            protosDb.TryFindProtoIgnoreCase("ProgramableNetwork_Module_Runtime_Delay_8", out ModuleProto proto);

            EntityManager manager = new EntityManager();
            EntityContext entityContext = new EntityContext(null, manager, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            FakeProto fakeProto = new FakeProto(new FakeProto.ID("fake"));
            manager.AddEntity(1, new FakeEntity(new EntityId(1), fakeProto, entityContext));
            Module m = new Module(proto, entityContext, null);

            Fix32 i = new Fix32();
            int[] outputs = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            while(true)
            {
                i += 1;
                m.Input["0"] = i;
                m.Execute();

                IEnumerable<string> converted = outputs.Select(o => m.Output[o.ToString(), Fix32.Zero].ToString());
                Console.WriteLine($"{i}, {string.Join(", ", converted)} [{m.Status}]");

                Console.WriteLine("lala");
            }
        }

        private static void InitBaseGame(ProtoRegistrator registrator)
        {
            registrator.PrototypesDb.Add(new ProductProto(new ProductProto.ID("Product_Electronics"), new Proto.Str(), 1.Quantity(), true, true, false, new ProductProto.Gfx(Option.None, Option.Create("text"))));
            registrator.PrototypesDb.Add(new VirtualProductProto(new ProductProto.ID("Product_Virtual_MaintenanceT1"), new Proto.Str(), new ProductProto.Gfx(Option.None, Option.Create("text"))));
        }
    }
}
