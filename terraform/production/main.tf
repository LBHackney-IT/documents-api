provider "aws" {
    region  = "eu-west-2"
    version = "~> 3.0"
}
data "aws_caller_identity" "current" {}

data "aws_region" "current" {}

data "aws_vpc" "dr_vpc" {
    tags = {
        Name = "disaster-recovery-prod"
    }
}

terraform {
    backend "s3" {
        bucket  = "terraform-state-disaster-recovery"
        encrypt = true
        region  = "eu-west-2"
        key     = "services/des/state"
    }
}

data "aws_ssm_parameter" "documents_postgres_port_security_group" {
    name = "/documents-api/production/postgres-port"
}

resource "aws_security_group" "documents_api_db_traffic" {
    vpc_id      = data.aws_vpc.dr_vpc.id
    name_prefix = "allow_documents_api_db_traffic"

    ingress {
        description = "documents_api_db_dr"
        from_port   = data.aws_ssm_parameter.documents_postgres_port_security_group.value
        to_port     = data.aws_ssm_parameter.documents_postgres_port_security_group.value
        protocol    = "tcp"

        cidr_blocks = [data.aws_vpc.dr_vpc.cidr_block]
    }

    egress {
        description = "allow outbound traffic"
        from_port   = 0
        to_port     = 0
        protocol    = "-1"
        cidr_blocks = ["0.0.0.0/0"]
    }

    tags = {
            "Name" = "documents_api_db_traffic-dr"
        }
}
