export interface Message {
  id: number
  senderUsername: string
  senderId: number
  senderPhotoUrl: string
  recipientId: number
  recipientPhotoUrl: string
  recipientUsername: string
  content: string
  dateRead?: Date
  messageSent: Date
}
