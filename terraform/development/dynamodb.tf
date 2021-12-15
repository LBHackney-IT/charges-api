resource "aws_dynamodb_table" "chargesapi_dynamodb_table" {
    name                  = "Charges"
    billing_mode          = "PROVISIONED"
    read_capacity         = 10
    write_capacity        = 10
    hash_key              = "target_id"
    sort_key              = "id" 
    attribute {
        name              = "id"
        type              = "S"
    }
	
    attribute {
        name              = "target_id"
        type              = "S"
    }

    tags = {
        Name              = "charges-api-${var.environment_name}"
        Environment       = var.environment_name
        terraform-managed = true
        project_name      = var.project_name
    }

    point_in_time_recovery {
        enabled           = true
    }
}
