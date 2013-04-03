﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using Umbraco.Core.Configuration;
using Umbraco.Core.IO;

namespace Umbraco.Core.Models
{
    /// <summary>
    /// Represents a Template file
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class Template : File, ITemplate
    {
        private readonly string _alias;
        private readonly string _name;
        private int _creatorId;
        private int _level;
        private int _sortOrder;
        private int _parentId;
        private string _nodePath;
        private int _masterTemplateId;
        private string _masterTemplateAlias;

        private static readonly PropertyInfo CreatorIdSelector = ExpressionHelper.GetPropertyInfo<Template, int>(x => x.CreatorId);
        private static readonly PropertyInfo LevelSelector = ExpressionHelper.GetPropertyInfo<Template, int>(x => x.Level);
        private static readonly PropertyInfo SortOrderSelector = ExpressionHelper.GetPropertyInfo<Template, int>(x => x.SortOrder);
        private static readonly PropertyInfo ParentIdSelector = ExpressionHelper.GetPropertyInfo<Template, int>(x => x.ParentId);
        private static readonly PropertyInfo NodePathSelector = ExpressionHelper.GetPropertyInfo<Template, string>(x => x.NodePath);
        //private static readonly PropertyInfo MasterTemplateIdSelector = ExpressionHelper.GetPropertyInfo<Template, int>(x => x.MasterTemplateId);
        private static readonly PropertyInfo MasterTemplateAliasSelector = ExpressionHelper.GetPropertyInfo<Template, string>(x => x.MasterTemplateAlias);
        

        internal Template(string path)
            : base(path)
        {
            base.Path = path;
            ParentId = -1;
        }

        public Template(string path, string name, string alias)
            : base(path)
        {
            base.Path = path;
            ParentId = -1;
            Key = name.EncodeAsGuid();
            _name = name.Replace("/", ".").Replace("\\", "");
            _alias = alias.ToSafeAlias();

            CreateDate = DateTime.Now;
            UpdateDate = DateTime.Now;
        }

        [DataMember]
        internal int CreatorId
        {
            get { return _creatorId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _creatorId = value;
                    return _creatorId;
                }, _creatorId, CreatorIdSelector);    
            }
        }

        [DataMember]
        internal int Level
        {
            get { return _level; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _level = value;
                    return _level;
                }, _level, LevelSelector);    
            }
        }

        [DataMember]
        internal int SortOrder
        {
            get { return _sortOrder; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _sortOrder = value;
                    return _sortOrder;
                }, _sortOrder, SortOrderSelector);    
            }
        }

        [DataMember]
        internal int ParentId
        {
            get { return _parentId; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _parentId = value;
                    return _parentId;
                }, _parentId, ParentIdSelector);    
            }
        }

        [DataMember]
        internal string NodePath
        {
            get { return _nodePath; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _nodePath = value;
                    return _nodePath;
                }, _nodePath, NodePathSelector);    
            }
        }

        [DataMember]
        internal Lazy<int> MasterTemplateId { get; set; }

        [DataMember]
        internal string MasterTemplateAlias
        {
            get { return _masterTemplateAlias; }
            set
            {
                SetPropertyValueAndDetectChanges(o =>
                {
                    _masterTemplateAlias = value;
                    return _masterTemplateAlias;
                }, _masterTemplateAlias, MasterTemplateAliasSelector);    
            }
        }

        [DataMember]
        public override string Alias
        {
            get
            {
                return _alias;
            }
        }

        [DataMember]
        public override string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Returns the <see cref="RenderingEngine"/> that corresponds to the template file
        /// </summary>
        /// <returns><see cref="RenderingEngine"/></returns>
        public RenderingEngine GetTypeOfRenderingEngine()
        {
            if(Path.EndsWith("cshtml") || Path.EndsWith("vbhtml"))
                return RenderingEngine.Mvc;

            return RenderingEngine.WebForms;
        }

        /// <summary>
        /// Boolean indicating whether the file could be validated
        /// </summary>
        /// <returns>True if file is valid, otherwise false</returns>
        public override bool IsValid()
        {
            var exts = new List<string>();
            if (UmbracoSettings.DefaultRenderingEngine == RenderingEngine.Mvc)
            {
                exts.Add("cshtml");
                exts.Add("vbhtml");
            }
            else
            {
                exts.Add(UmbracoSettings.UseAspNetMasterPages ? "master" : "aspx");
            }

            var dirs = SystemDirectories.Masterpages;
            if (UmbracoSettings.DefaultRenderingEngine == RenderingEngine.Mvc)
                dirs += "," + SystemDirectories.MvcViews;

            //Validate file
            var validFile = IOHelper.VerifyEditPath(Path, dirs.Split(','));

            //Validate extension
            var validExtension = IOHelper.VerifyFileExtension(Path, exts);

            return validFile && validExtension;
        }
    }
}