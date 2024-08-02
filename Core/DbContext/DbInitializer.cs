using GerberBackend.Core.Entities.Gerber.Elements;
using Microsoft.EntityFrameworkCore;

namespace GerberBackend.Core.DbContext;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        SeedData(scope.ServiceProvider.GetService<ApplicationContext>());
    }

    private static void SeedData(ApplicationContext context)
    {
        context.Database.Migrate();

        if (context.BaseMaterials.Any() 
            && context.BoardThickness.Any() 
            && context.ContourMachinings.Any() 
            && context.DataNumberings.Any() 
            && context.DrillFiles.Any() 
            && context.Layers.Any() 
            && context.MarkingColors.Any() 
            && context.MaskColors.Any()
            && context.MaskSides.Any()
            && context.MarkingSides.Any()
            && context.MaskTypes.Any()
            && context.Vias.Any()
            && context.MinimalConductors.Any()
            && context.MainSites.Any()
            && context.EdgeConnectors.Any()
            && context.AngleChamfers.Any())
        {
            Console.WriteLine("Данные уже существуют!");
            return;
        }

        var baseMaterial = new List<BaseMaterial>()
        {
            new BaseMaterial
            {
                IsActive = true,
                Value = "FR-4",
                Identity = 0,
            },
            new BaseMaterial
            {
                IsActive = false,
                Value = "Flex",
                Identity = 1,
            },
            new BaseMaterial
            {
                IsActive = false,
                Value = "Aluminum",
                Identity = 2,
               
            },
            new BaseMaterial
            {
                IsActive = false,
                Value = "Copper Core",
                Identity = 3,
            },
            new BaseMaterial
            {
                IsActive = false,
                Value = "Rogers",
                Identity = 4,
            },
            new BaseMaterial
            {
                IsActive = false,
                Value = "PTFE Teflon",
                Identity = 5,
            }
        };

        var layers = new List<Layer>()
        {
            new Layer
            {
                IsActive = true,
                Value = "1",
                Identity = 0,
            },
            new Layer
            {
                IsActive = true,
                Value = "2",
                Identity = 1,
            }
        };

        var boardThickness = new List<BoardThickness>()
        {
            new BoardThickness
            {
                IsActive = true,
                Value= "0.1",
                Identity = 0,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "0.15",
                Identity = 1,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "0.2",
                Identity = 2,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "0.25",
                Identity = 3,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "0.36",
                Identity = 4,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "0.5",
                Identity = 5,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "0.71",
                Identity = 6,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "1",
                Identity = 7,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "1.5",
                Identity = 8,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "2",
                Identity = 9,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "2.4",
                Identity = 10,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "2.5",
                Identity = 11,
            },
            new BoardThickness
            {
                IsActive = true,
                Value = "3",
                Identity = 12,
            }
        };

        var foilThickness = new List<FoilThickness>()
        {
            new FoilThickness
            {
                IsActive = true,
                Value = "18",
                Identity = 0,
            },
            new FoilThickness
            {
                IsActive = true,
                Value = "35",
                Identity = 1,
            },
            new FoilThickness
            {
                IsActive = true,
                Value = "50",
                Identity = 2,
            },
            new FoilThickness
            {
                IsActive = true,
                Value = "70",
                Identity = 3,
            },
            new FoilThickness
            {
                IsActive= true,
                Value = "105",
                Identity = 4,
            }
        };

        var maskColor = new List<MaskColor>()
        {
            new MaskColor
            {
                IsActive= true,
                Value = "Зеленый",
                Identity = 0,
            },
            new MaskColor
            {
                IsActive= true,
                Value = "Пурпурный",
                Identity = 1,
            },
            new MaskColor
            {
                IsActive = true,
                Value = "Красный",
                Identity = 2,
            },
            new MaskColor
            {
                IsActive= true,
                Value = "Желтый",
                Identity = 3,
            },
            new MaskColor
            {
                IsActive= true,
                Value = "Синий",
                Identity = 4,
            },
            new MaskColor
            {
                IsActive= true,
                Value = "Белый",
                Identity = 5,
            },
            new MaskColor
            {
                IsActive= true,
                Value = "Черный",
                Identity = 6,
            }
        };

        var markingColor = new List<MarkingColor>()
        {
            new MarkingColor
            {
                IsActive= true,
                Value = "Белый",
                Identity = 0,
            },
            new MarkingColor
            {
                IsActive= true,
                Value = "Черный",
                Identity = 1,
            }
        };

        var dataNumbering = new List<DataNumbering>()
        {
            new DataNumbering
            {
                IsActive = true,
                Value = "Нет",
                Identity = 0,
            },
            new DataNumbering
            {
                IsActive= true,
                Value = "Дата",
                Identity = 1,
            },
            new DataNumbering
            {
                IsActive= true,
                Value = "Сквозная нумерация",
                Identity = 2,
            }
        };

        var contourMachining = new List<ContourMachining>()
        {
            new ContourMachining
            {
                IsActive = true,
                Value = "Фрезеровка",
                Identity = 0,
            },
            new ContourMachining
            {
                IsActive = true,
                Value = "Скрайбирование",
                Identity = 1,
            },
            new ContourMachining
            {
                IsActive= true,
                Value = "Фрезеровка и скрайбирование",
                Identity = 2,
            }
        };

        var drillFile = new List<DrillFile>()
        {
            new DrillFile
            {
                IsActive = true,
                Value = "Свёрла",
                Identity = 0,
            },
            new DrillFile
            {
                IsActive = true,
                Value = "Конечного отверстия",
                Identity = 1,
            }
        };

        var maskType = new List<MaskType>()
        {
            new MaskType
            {
                IsActive = true,
                Value = "Нет",
                Identity = 0,
            },
            new MaskType
            {
                IsActive = true,
                Value = "Жидная ПМ",
                Identity = 1,
            },
            new MaskType
            {
                IsActive = true,
                Value = "Пленочная ПМ",
                Identity = 2,
            }
        };

        var maskSide = new List<MaskSide>()
        {
            new MaskSide
            {
                IsActive = true,
                Value = "Верх",
                Identity = 0,
            },
            new MaskSide
            {
                IsActive = true,
                Value = "Низ",
                Identity = 1,
            },
            new MaskSide
            {
                IsActive = true,
                Value = "С двух сторон",
                Identity = 2,
            }
        };

        var markingSide = new List<MarkingSide>()
        {
            new MarkingSide
            {
                IsActive = true,
                Value = "Нет",
                Identity = 0,
            },
            new MarkingSide
            {
                IsActive = true,
                Value = "С двух сторон",
                Identity = 1,
            },
            new MarkingSide
            {
                IsActive = true,
                Value = "Верх",
                Identity = 2,
            },
            new MarkingSide
            {
                IsActive = true,
                Value = "Низ",
                Identity = 3,
            }
        };

        var vias = new List<Vias>()
        {
            new Vias
            {
                IsActive = true,
                Value = "Переходные отверстия закрыты маской",
                Identity = 0,
            },
            new Vias
            {
                IsActive = true,
                Value = "Переходные отверстия открыты от маски",
                Identity = 1,

            }
        };

        var minimalConductor = new List<MinimalConductor>()
        {
            new MinimalConductor
            {
                IsActive = true,
                Value = "больше или равно 0.125 / 0.125 / d 0.2",
                Identity = 0,
            },
            new MinimalConductor
            {
                IsActive = true,
                Value = "больше или равно 0.15 / 0.15 / d 0.3",
                Identity = 1,
            },
            new MinimalConductor
            {
                IsActive = true,
                Value = "менее 0.125 / 0.125 / d 0.25",
                Identity = 2,
            },
            new MinimalConductor
            {
                IsActive = true,
                Value = "от 0.2 / 0.2 / d 0.4 и больше",
                Identity = 3,
            }
        };

        var edgeConnector = new List<EdgeConnectors>()
        {
            new EdgeConnectors
            {
                IsActive = true,
                Value = "Нет",
                Identity = 0,
            },
            new EdgeConnectors
            {
                IsActive = true,
                Value = "основное покрытие",
                Identity = 1,
            },
            new EdgeConnectors
            {
                IsActive = true,
                Value = "Гальваническое Au",
                Identity = 2,
            },
            new EdgeConnectors
            {
                IsActive = true,
                Value = "Гальванический Ni",
                Identity = 3,
            }
        };

        var mainSites = new List<MainSites>()
        {
            new MainSites
            {
                IsActive = true,
                Value = "Иммерсионный никель",
                Identity = 0,
            },
            new MainSites
            {
                IsActive = true,
                Value = "Иммерсионное золочение",
                Identity = 1,
            },
            new MainSites
            {
                IsActive = true,
                Value = "Иммерсионное олово",
                Identity = 2,
            },
            new MainSites
            {
                IsActive = true,
                Value = "Горячее лужение ПОС-63 (HASL)",
                Identity = 3,
            },
            new MainSites
            {
                IsActive = true,
                Value = "Отсутствует (голая медь)",
                Identity = 4,
            }
        };

        var angleChamer = new List<AngleChamfer>()
        {
            new AngleChamfer
            {
                IsActive = true,
                Value = "Нет",
                Identity = 0,
            },
            new AngleChamfer
            {
                IsActive = true,
                Value = "45",
                Identity = 1,
            },
            new AngleChamfer
            {
                IsActive = true,
                Value = "20",
                Identity = 2,
            }
        };

        context.BaseMaterials.AddRange(baseMaterial);
        context.Layers.AddRange(layers);
        context.BoardThickness.AddRange(boardThickness);
        context.FoilThicknesses.AddRange(foilThickness);
        context.MaskColors.AddRange(maskColor);
        context.MarkingColors.AddRange(markingColor);
        context.DataNumberings.AddRange(dataNumbering);
        context.ContourMachinings.AddRange(contourMachining);
        context.DrillFiles.AddRange(drillFile);
        context.MaskTypes.AddRange(maskType);
        context.MaskSides.AddRange(maskSide);
        context.MarkingSides.AddRange(markingSide);
        context.MainSites.AddRange(mainSites);
        context.Vias.AddRange(vias);
        context.EdgeConnectors.AddRange(edgeConnector);
        context.MinimalConductors.AddRange(minimalConductor);
        context.AngleChamfers.AddRange(angleChamer);

        context.SaveChanges();
    }
}
