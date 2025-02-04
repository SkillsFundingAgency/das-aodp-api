CREATE TABLE [regulated].[qualification_versions](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[qualification_id] [int] NOT NULL,
	[version_field_changes_id] [int] NOT NULL,
	[process_status_id] [int] NOT NULL,
	[additional_key_changes_received_flag] [int] NOT NULL,
	[lifecycle_stage_id] [int] NOT NULL,
	[outcome_justification_notes] [varchar](max) NULL,
	[organisation_id] [int] NOT NULL,
	[status] [varchar](100) NOT NULL,
	[type] [varchar](50) NOT NULL,
	[ssa] [varchar](150) NOT NULL,
	[level] [varchar](50) NOT NULL,
	[sub_level] [varchar](50) NOT NULL,
	[eqf_level] [varchar](50) NOT NULL,
	[grading_type] [varchar](50) NULL,
	[grading_scale] [varchar](250) NULL,
	[total_credits] [int] NULL,
	[tqt] [int] NULL,
	[glh] [int] NULL,
	[minimum_glh] [int] NULL,
	[maximum_glh] [int] NULL,
	[regulation_start_date] [datetime] NOT NULL,
	[operational_start_date] [datetime] NOT NULL,
	[operational_end_date] [datetime] NULL,
	[certification_end_date] [datetime] NULL,
	[review_date] [datetime] NULL,
	[offered_in_england] [bit] NOT NULL,
	[offered_in_ni] [bit] NOT NULL,
	[offered_internationally] [bit] NULL,
	[specialism] [varchar](max) NULL,
	[pathways] [varchar](max) NULL,
	[assessment_methods] [varchar](max) NULL,
	[approved_for_del_funded_programme] [varchar](150) NULL,
	[link_to_specification] [varchar](max) NULL,
	[apprenticeship_standard_reference_number] [varchar](50) NULL,
	[apprenticeship_standard_title] [varchar](150) NULL,
	[regulated_by_northern_ireland] [bit] NOT NULL,
	[ni_discount_code] [varchar](150) NULL,
	[gce_size_equivelence] [varchar](50) NULL,
	[gcse_size_equivelence] [varchar](50) NULL,
	[entitlement_framework_design] [varchar](50) NULL,
	[last_updated_date] [datetime] NOT NULL,
	[ui_last_updated_date] [datetime] NOT NULL,
	[inserted_date] [datetime] NOT NULL,
	[version] [int] NULL,
	[appears_on_public_register] [bit] NULL,
	[level_id] [int] NULL,
	[type_id] [int] NULL,
	[ssa_id] [int] NULL,
	[grading_type_id] [int] NULL,
	[grading_scale_id] [int] NULL,
	[pre_sixteen] [bit] NULL,
	[sixteen_to_eighteen] [bit] NULL,
	[eighteen_plus] [bit] NULL,
	[nineteen_plus] [bit] NULL,
	[import_status] [varchar](50) NULL, 
    CONSTRAINT [PK_qualification_versions] PRIMARY KEY ([id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [regulated].[qualification_versions]  WITH CHECK ADD FOREIGN KEY([lifecycle_stage_id])
REFERENCES [regulated].[lifecycle_stage] ([id])
GO
ALTER TABLE [regulated].[qualification_versions]  WITH CHECK ADD FOREIGN KEY([lifecycle_stage_id])
REFERENCES [regulated].[lifecycle_stage] ([id])
GO
ALTER TABLE [regulated].[qualification_versions]  WITH CHECK ADD FOREIGN KEY([organisation_id])
REFERENCES [dbo].[awarding_organisation] ([id])
GO
ALTER TABLE [regulated].[qualification_versions]  WITH CHECK ADD FOREIGN KEY([process_status_id])
REFERENCES [regulated].[process_status] ([id])
GO
ALTER TABLE [regulated].[qualification_versions]  WITH CHECK ADD FOREIGN KEY([qualification_id])
REFERENCES [dbo].[qualification] ([id])
GO
ALTER TABLE [regulated].[qualification_versions]  WITH CHECK ADD FOREIGN KEY([version_field_changes_id])
REFERENCES [regulated].[version_field_changes] ([id])
GO
