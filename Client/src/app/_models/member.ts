import { Photo } from "./photo"

export interface Member {
  id: number
  username: string
  photoUrl: string
  age: string
  knownAs: string
  createdAt: Date
  lastActive: Date
  gender: string
  introduction: string
  interests: string
  lookingFor: string
  city: string
  country: string
  photos: Photo[] 
}


