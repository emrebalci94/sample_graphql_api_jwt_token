Sample graphql api

Sample Queries:

Query 1:

{"query":
 "query{ 
 	 Materials{
	Brand{
		Name
	}
 	Name
 	}
  }" 
}

Query 2:

{"query":
 "query
 
 	getMeterialByBrandId($Id:Int!){
 	MeterialByBrandId(Id:$Id){
 		Name
 		Brand{
 			Id
 			Name
 		}
 	}
 
  }" ,
   	"variables":{
   		"Id":8
   	}
}

Query 3:

{"query":
 "query
 	getMeterialByBrandId($Id:Int!){
 	MeterialByBrandId(Id:$Id){
 		Name
 		Brand{
 			Id
 			Name
 		}
 	}
 	Brands{
 		Name
 	}
  }" ,
   	"variables":{
   		"Id":12
   	}
}

