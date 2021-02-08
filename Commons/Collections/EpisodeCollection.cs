﻿using Commons.Filters;
using MongoDB.Driver;
using MongoService;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Collections
{
    public class EpisodeCollection : ICollection<Episode>
    {
        protected override string CollectionName => "episode";

        public override void Add(ref Episode document)
        {
            document.Id = this.CalcNewId();
            document.CreationDate = DateTime.Now;
            document.UpdateDate = null;

            this.Collection.InsertOne(document);
        }

        public override long Count()
        {
            return this.Collection.CountDocuments(Builders<Episode>.Filter.Empty);
        }

        public override void Delete(long id)
        {
            throw new NotImplementedException();
        }

        public override void Edit(ref Episode document)
        {
            document.UpdateDate = DateTime.Now;

            var filter = Builders<Episode>.Filter.Eq("_id", document.Id);
            this.Collection.ReplaceOne(filter, document);
        }

        public override bool Exists(ref Episode document, bool updateValues = true)
        {
            long animeId = document.AnimeID;
            int number = document.Number;

            Episode reference = this.Collection.Find(x => x.AnimeID == animeId && x.Number == number).FirstOrDefault();

            if (reference != null)
            {
                if (updateValues)
                {
                    document.Id = reference.Id;
                    document.CreationDate = reference.CreationDate;
                    document.UpdateDate = reference.UpdateDate;
                }
                return true;
            }

            return false;
        }

        public override Episode Get(long id)
        {
            return this.Collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public override Paging<Episode> GetList<TFilter>(IFilter<TFilter> filter)
        {
            EpisodeFilter episodeFilter = filter as EpisodeFilter;

            var builder = Builders<Episode>.Filter;
            FilterDefinition<Episode> queryFilter = builder.Empty;

            if(episodeFilter.anime_id > 0)
            {
                queryFilter = queryFilter & builder.Eq("anime_id", episodeFilter.anime_id);
            }

            if (!string.IsNullOrEmpty(episodeFilter.source))
            {
                queryFilter = queryFilter & builder.Regex($"source", episodeFilter.source);
            }

            return new Paging<Episode>(this.Collection, episodeFilter.page, queryFilter, episodeFilter.per_page);
        }
    }
}