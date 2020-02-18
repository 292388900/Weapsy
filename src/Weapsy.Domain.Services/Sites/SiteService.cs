﻿using System;
using System.Threading.Tasks;
using FluentValidation;
using Kledex.Commands;
using Microsoft.EntityFrameworkCore;
using Weapsy.Data;
using Weapsy.Domain.Models.Sites;
using Weapsy.Domain.Models.Sites.Commands;
using Weapsy.Domain.Models.Sites.Events;

namespace Weapsy.Domain.Services.Sites
{
    public class SiteService : ISiteService
    {
        private readonly WeapsyDbContext _dbContext;
        private readonly IValidator<CreateSite> _createSiteValidator;
        private readonly IValidator<UpdateSite> _updateSiteValidator;

        public SiteService(WeapsyDbContext dbContext,
            IValidator<CreateSite> createSiteValidator, 
            IValidator<UpdateSite> updateSiteValidator)
        {
            _dbContext = dbContext;
            _createSiteValidator = createSiteValidator;
            _updateSiteValidator = updateSiteValidator;
        }

        public async Task<CommandResponse> CreateAsync(CreateSite command)
        {
            await _createSiteValidator.ValidateAndThrowAsync(command);

            var site = new Site(command);

            _dbContext.Sites.Add(site);

            await _dbContext.SaveChangesAsync();

            return new CommandResponse(new SiteCreated
            {
                Name = site.Name
            });
        }

        public async Task<CommandResponse> UpdateAsync(UpdateSite command)
        {
            var site = await _dbContext.Sites.FirstAsync(x => x.Id == command.Id);

            if (site == null)
            {
                throw new ApplicationException($"Site with Id {command.Id} not found.");
            }

            await _updateSiteValidator.ValidateAndThrowAsync(command);

            site.Update(command);

            await _dbContext.SaveChangesAsync();

            return new CommandResponse(new SiteUpdated
            {
                Id = site.Id
            });
        }
    }
}