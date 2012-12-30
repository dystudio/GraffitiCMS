using System;
using System.Collections.Generic;
using System.Linq;
using Graffiti.Core.Services;

namespace Graffiti.Core
{
    public class TagWeightCloud
    {

        private static ITagService _tagService = ServiceLocator.Get<ITagService>();

        public static TagWeightCollection FetchTags(int minWeight, int maxItems)
        {
            TagWeightCollection twc = ZCache.Get<TagWeightCollection>("TagCloud:" + minWeight + "|" + maxItems);
            if (twc == null)
            {
                IList<TagWeight> tagWeights = _tagService.FetchAllTagWeights();

                if (minWeight > 0)
                    tagWeights = tagWeights.Where(x => x.Weight >= minWeight).ToList();

                if (maxItems > 0)
                    tagWeights = tagWeights.Take(maxItems).ToList();

                //q.OrderByAsc(TagWeight.Columns.Weight);
                tagWeights = tagWeights.OrderByDescending(x => x.Weight).ToList();

                twc = new TagWeightCollection(tagWeights);

                double mean = 0;
                double std = StdDev(twc, out mean);

                foreach (TagWeight tag in twc)
                {
                    tag.FontFactor = NormalizeWeight(tag.Count, mean, std);
                    tag.FontSize = _fontSizes[tag.FontFactor - 1];
                }

                twc.Sort(delegate(TagWeight t1, TagWeight t2) {return Comparer<string>.Default.Compare(t1.Name, t2.Name); });

                ZCache.InsertCache("TagCloud:" + minWeight + "|" + maxItems,twc,120);
            }

            return twc;
        }

        static string[] _fontSizes = new string[] { "xx-small", "x-small", "small", "medium", "large", "x-large", "xx-large" };

        private static int NormalizeWeight(double weight, double mean, double stdDev)
        {
            double factor = (weight - mean);

            if (factor != 0 && stdDev != 0) factor /= stdDev;

            return (factor > 2) ? 7 :
                   (factor > 1) ? 6 :
                   (factor > 0.5) ? 5 :
                   (factor > -0.5) ? 4 :
                   (factor > -1) ? 3 :
                   (factor > -2) ? 2 :
                   1;
        }


        private static double Mean(TagWeightCollection twc)
        {
            double sum = 0;
            int count = 0;

            foreach (TagWeight item in twc)
            {
                sum += item.Weight;
                count++;
            }

            return sum / count;
        }

        private static double StdDev(TagWeightCollection twc, out double mean)
        {
            mean = Mean(twc);
            double sumOfDiffSquares = 0;
            int count = 0;

            foreach (TagWeight item in twc)
            {
                double diff = (item.Weight - mean);
                sumOfDiffSquares += diff * diff;
                count++;
            }

            return Math.Sqrt(sumOfDiffSquares / count);
        }
    }
}